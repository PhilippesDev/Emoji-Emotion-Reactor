using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Dnn;
using Emgu.CV.Structure;

namespace reactor
{
    public partial class Form1 : Form
    {
        private readonly Dictionary<int, (string name, string gif)> Emotions = new()
        {
            {0, ("Colère", "angry.gif")},
            {1, ("Dégoût", "disgust.gif")},
            {2, ("Peur", "fear.gif")},
            {3, ("Heureux", "happy.gif")},
            {4, ("Triste", "sad.gif")},
            {5, ("Surpris", "surprise.gif")},
            {6, ("Neutre", "neutral.gif")}
        };

        private VideoCapture _capture;
        private CascadeClassifier _faceCascade;
        private Net _emotionNet;
        private Net _ageNet;
        private Net _genderNet;
        private System.Windows.Forms.Timer _cameraTimer;
        private bool _cameraOn = false;
        private readonly Dictionary<int, Image> _emotionGifs = new Dictionary<int, Image>();
        private readonly object _lock = new object();
        private readonly List<int> _emotionHistory = new List<int>(); // Pour lisser les prédictions

        public Form1()
        {
            InitializeComponent();
            SetupApplication();
        }

        private void SetupApplication()
        {
            try
            {
                CvInvoke.UseOpenCL = false; 

             
                _capture = new VideoCapture(0);
                if (!_capture.IsOpened)
                    throw new Exception("Impossible d'ouvrir la webcam.");

                if (!File.Exists("haarcascade_frontalface_default.xml"))
                    throw new FileNotFoundException("Fichier haarcascade_frontalface_default.xml introuvable.");
                if (!File.Exists("emotion_model.onnx"))
                    throw new FileNotFoundException("Fichier emotion_model.onnx introuvable.");
                if (!File.Exists("age_deploy.prototxt") || !File.Exists("age_net.caffemodel"))
                    throw new FileNotFoundException("Fichiers du modèle d'âge introuvables.");
                if (!File.Exists("gender_deploy.prototxt") || !File.Exists("gender_net.caffemodel"))
                    throw new FileNotFoundException("Fichiers du modèle de genre introuvables.");

                _faceCascade = new CascadeClassifier("haarcascade_frontalface_default.xml")
                    ?? throw new Exception("Erreur de chargement du modèle de détection de visage.");
                _emotionNet = DnnInvoke.ReadNetFromONNX("emotion_model.onnx")
                    ?? throw new Exception("Erreur de chargement du modèle d'émotion.");
                _ageNet = DnnInvoke.ReadNetFromCaffe("age_deploy.prototxt", "age_net.caffemodel")
                    ?? throw new Exception("Erreur de chargement du modèle d'âge.");
                _genderNet = DnnInvoke.ReadNetFromCaffe("gender_deploy.prototxt", "gender_net.caffemodel")
                    ?? throw new Exception("Erreur de chargement du modèle de genre.");

                LoadEmotionGifs();

           
                _cameraTimer = new System.Windows.Forms.Timer { Interval = 100 };
                _cameraTimer.Tick += (s, e) => ProcessFrame();

                
                button1.Click += ToggleCamera;
                button2.Click += DetectAge;
                button3.Click += DetectGender;

                Streamcamera.BackgroundImageLayout = ImageLayout.Zoom;
                emojiBox.BackgroundImageLayout = ImageLayout.Zoom;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erreur d'initialisation: {ex.Message}", "Erreur",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void LoadEmotionGifs()
        {
            foreach (var kvp in Emotions)
            {
                string path = Path.Combine("emojis", kvp.Value.gif);
                if (File.Exists(path))
                {
                    _emotionGifs[kvp.Key] = Image.FromFile(path);
                }
                else
                {
                    MessageBox.Show($"GIF introuvable: {path}", "Avertissement", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void ToggleCamera(object sender, EventArgs e)
        {
            _cameraOn = !_cameraOn;
            button1.Text = _cameraOn ? "Couper la caméra" : "Démarrer la caméra";

            if (_cameraOn)
            {
                _capture.Start();
                _cameraTimer.Start();
            }
            else
            {
                _cameraTimer.Stop();
                _capture.Stop();
                UpdateUI(() =>
                {
                    Streamcamera.BackgroundImage = null;
                    emojiBox.BackgroundImage = null;
                    Result.Text = "Aucun visage détecté";
                    Result.ForeColor = Color.White;
                });
            }
        }

        private float[] ApplySoftmax(float[] logits)
        {
            float maxLogit = logits.Max(); 
            float sum = 0f;
            float[] probabilities = new float[logits.Length];

            for (int i = 0; i < logits.Length; i++)
            {
                probabilities[i] = (float)Math.Exp(logits[i] - maxLogit);
                sum += probabilities[i];
            }

            for (int i = 0; i < probabilities.Length; i++)
            {
                probabilities[i] /= sum;
            }

            return probabilities;
        }

        private void ProcessFrame()
        {
            if (!_cameraOn) return;

            lock (_lock)
            {
                using (var frame = new Mat())
                {
                    try
                    {
                        _capture.Retrieve(frame);
                        if (frame.IsEmpty) return;

                        using (var gray = new Mat())
                        {
                            CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);

                          
                            var faces = _faceCascade.DetectMultiScale(gray, 1.3, 5, minSize: new Size(60, 60));
                            if (faces.Length > 0)
                            {
                                var face = faces[0];
                                using (var faceImg = new Mat(gray, face))
                                using (var resized = new Mat())
                                {
                                    CvInvoke.Resize(faceImg, resized, new Size(48, 48));
                                  
                                    CvInvoke.Imwrite("debug_face.jpg", resized);

                       
                                    using (var blob = DnnInvoke.BlobFromImage(resized, 1.0 / 255.0, new Size(48, 48), new MCvScalar(0, 0, 0), false))
                                    {
                                        _emotionNet.SetInput(blob);
                                        using (var output = _emotionNet.Forward())
                                        {
                                          
                                            if (output.Cols != 7 || output.Rows != 1)
                                            {
                                                UpdateUI(() =>
                                                {
                                                    Result.Text = $"Sortie invalide: [{output.Rows}, {output.Cols}]";
                                                    Result.ForeColor = Color.Red;
                                                    emojiBox.BackgroundImage = null;
                                                });
                                                return;
                                            }

                                            float[] scores = new float[7];
                                            output.CopyTo(scores);

                                        
                                            float[] probabilities = ApplySoftmax(scores);
                                            int emotionId = Array.IndexOf(probabilities, probabilities.Max());
                                            float maxScore = probabilities.Max();

                                            Console.WriteLine($"Raw Scores: {string.Join(", ", scores.Select(s => s.ToString("F4")))}");
                                            Console.WriteLine($"Probabilities: {string.Join(", ", probabilities.Select(p => p.ToString("F4")))}, Max: {maxScore:F4}, Emotion: {emotionId}");

                                           
                                            _emotionHistory.Add(emotionId);
                                            if (_emotionHistory.Count > 5) _emotionHistory.RemoveAt(0);

                                            var mostFrequent = _emotionHistory.GroupBy(x => x)
                                                .OrderByDescending(g => g.Count())
                                                .First().Key;

                                            if (Emotions.ContainsKey(mostFrequent))
                                            {
                                                DisplayEmotion(mostFrequent, maxScore);
                                            }
                                            else
                                            {
                                                UpdateUI(() =>
                                                {
                                                    Result.Text = $"Émotion {mostFrequent} non trouvée";
                                                    Result.ForeColor = Color.Red;
                                                    emojiBox.BackgroundImage = null;
                                                });
                                            }
                                        }
                                    }
                                }

                                CvInvoke.Rectangle(frame, face, new MCvScalar(0, 255, 0), 2);
                            }
                            else
                            {
                                UpdateUI(() =>
                                {
                                    Result.Text = "Aucun visage détecté";
                                    Result.ForeColor = Color.White;
                                    emojiBox.BackgroundImage = null;
                                });
                            }

                            DisplayImage(frame);
                        }
                    }
                    catch (Exception ex)
                    {
                        UpdateUI(() =>
                        {
                            Result.Text = $"Erreur: {ex.Message}";
                            Result.ForeColor = Color.Red;
                        });
                    }
                }
            }
        }

        private void DisplayImage(Mat image)
        {
            var bitmap = image.ToBitmap();
            UpdateUI(() =>
            {
                var oldImage = Streamcamera.BackgroundImage;
                Streamcamera.Image = bitmap;
                oldImage?.Dispose();
            });
        }

        private void DisplayEmotion(int emotionId, float confidence)
        {
            UpdateUI(() =>
            {
                if (_emotionGifs.TryGetValue(emotionId, out var gif))
                {
                    emojiBox.Image?.Dispose();
                    emojiBox.Image = (Image)gif.Clone();
                    Result.Text = $"{Emotions[emotionId].name} (Score: {confidence:F2})";
                    Result.ForeColor = Color.LightGreen;
                }
                else
                {
                    Result.Text = $"Émotion {emotionId} non trouvée";
                    Result.ForeColor = Color.Red;
                }
            });
        }

        private void UpdateUI(Action action)
        {
            if (InvokeRequired)
                Invoke(action);
            else
                action();
        }

        private void DetectAge(object sender, EventArgs e)
        {
            if (!_cameraOn)
            {
                UpdateUI(() =>
                {
                    Result.Text = "Allumez la caméra d'abord";
                    Result.ForeColor = Color.Orange;
                });
                return;
            }

            using (var frame = _capture.QueryFrame())
            {
                if (frame == null || frame.IsEmpty) return;

                using (var gray = new Mat())
                {
                    CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
                    var faces = _faceCascade.DetectMultiScale(gray, 1.3, 5, minSize: new Size(60, 60));
                    if (faces.Length > 0)
                    {
                        var face = faces[0];
                        using (var faceImg = new Mat(frame, face))
                        using (var resized = new Mat())
                        {
                            CvInvoke.Resize(faceImg, resized, new Size(227, 227));
                            using (var blob = DnnInvoke.BlobFromImage(resized, 1.0, new Size(227, 227), new MCvScalar(78.4263377603, 87.7689143744, 114.895847746)))
                            {
                                _ageNet.SetInput(blob);
                                using (var output = _ageNet.Forward())
                                {
                                    float[] scores = new float[output.Cols];
                                    output.CopyTo(scores);
                                    int ageId = Array.IndexOf(scores, scores.Max());
                                    string[] ageRanges = { "(0-2)", "(4-6)", "(8-12)", "(15-20)", "(25-32)", "(38-43)", "(48-53)", "(60-100)" };
                                    string age = ageRanges[ageId];

                                    ShowCustomPopup($"Âge estimé: {age}", "Résultat de l'âge");
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowCustomPopup("Aucun visage détecté", "Erreur");
                    }
                }
            }
        }

        private void DetectGender(object sender, EventArgs e)
        {
            if (!_cameraOn)
            {
                UpdateUI(() =>
                {
                    Result.Text = "Allumez la caméra d'abord";
                    Result.ForeColor = Color.Orange;
                });
                return;
            }

            using (var frame = _capture.QueryFrame())
            {
                if (frame == null || frame.IsEmpty) return;

                using (var gray = new Mat())
                {
                    CvInvoke.CvtColor(frame, gray, ColorConversion.Bgr2Gray);
                    var faces = _faceCascade.DetectMultiScale(gray, 1.3, 5, minSize: new Size(60, 60));
                    if (faces.Length > 0)
                    {
                        var face = faces[0];
                        using (var faceImg = new Mat(frame, face))
                        using (var resized = new Mat())
                        {
                            CvInvoke.Resize(faceImg, resized, new Size(227, 227));
                            using (var blob = DnnInvoke.BlobFromImage(resized, 1.0, new Size(227, 227), new MCvScalar(78.4263377603, 87.7689143744, 114.895847746)))
                            {
                                _genderNet.SetInput(blob);
                                using (var output = _genderNet.Forward())
                                {
                                    float[] scores = new float[output.Cols];
                                    output.CopyTo(scores);
                                    int genderId = Array.IndexOf(scores, scores.Max());
                                    string gender = genderId == 0 ? "Masculin" : "Féminin";

                                    ShowCustomPopup($"Genre estimé: {gender}", "Résultat du genre");
                                }
                            }
                        }
                    }
                    else
                    {
                        ShowCustomPopup("Aucun visage détecté", "Erreur");
                    }
                }
            }
        }

        private void ShowCustomPopup(string message, string title)
        {
            var popup = new Form
            {
                Text = title,
                Size = new Size(300, 150),
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(30, 30, 30),
                ForeColor = Color.White
            };

            var label = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 12, FontStyle.Regular),
                AutoSize = true,
                Location = new Point(20, 30),
                ForeColor = Color.White
            };

            var button = new Button
            {
                Text = "OK",
                Size = new Size(80, 30),
                Location = new Point(110, 80),
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(0, 120, 215),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10)
            };
            button.FlatAppearance.BorderSize = 0;
            button.Click += (s, e) => popup.Close();

            popup.Controls.Add(label);
            popup.Controls.Add(button);
            popup.ShowDialog();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            _cameraTimer?.Stop();
            _capture?.Stop();
            _capture?.Dispose();
            _faceCascade?.Dispose();
            _emotionNet?.Dispose();
            _ageNet?.Dispose();
            _genderNet?.Dispose();

            foreach (var gif in _emotionGifs.Values)
            {
                gif.Dispose();
            }

            base.OnFormClosing(e);
        }
    }
}