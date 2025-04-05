using System;
using System.Timers;

namespace LabelPrinter.Logic
{
    public sealed class TextWaitingAnimation
    {
        private readonly string[] slashes = ["-", "\\", "|", "/"];
        private readonly string[] triangles = ["◸", "◹", "◺", "◻"];
        private readonly string[] clockCircles = ["◴", "◵", "◶", "◷"];
        private readonly string[] clockSquare = ["◰", "◳", "◲", "◱"];
        private readonly string[] blocksVertical = ["▁", "▂", "▃", "▄", "▅", "▆", "▇", "█"];
        private readonly string[] blocksHorizontal = ["▉", "▊", "▋", "▌", "▍", "▎", "▏"];
        private readonly string[] blockChars = ["▔", "▕", "▖", "▗", "▘", "▙", "▚", "▛", "▜", "▝", "▞", "▟"];
        private string[] animationParts;
        private Int16 animationIndex = 0;
        private AnimationTypes animationType;
        private readonly Timer timer;

        public enum AnimationTypes
        {
            Slashes,
            Triangles,
            ClockCircle,
            ClockSquare,
            BlocksVertical,
            BlocksHorizontal,
            BlockChars
        }

        public bool UseBrackets { get; set; } = true;
        public AnimationTypes AnimationType
        {
            get
            {
                return this.animationType;
            }

            set
            {
                this.animationType = value;
                this.SetAnimationType();
            }
        }
        public bool IsRunning
        {
            get
            {
                return this.timer.Enabled;
            }
        }
        public double Interval
        {
            get
            {
                return this.timer.Interval;
            }
            set
            {
                this.timer.Interval = value;
            }
        }

        public event EventHandler<string> AnimationChanged;

        #region Ctor
        public TextWaitingAnimation()
        {
            this.timer = new()
            {
                Interval = 1000,
            };

            this.timer.Elapsed += this.Timer_Tick;
        }
        #endregion

        private void Timer_Tick(object sender, ElapsedEventArgs e)
        {
            if (this.animationIndex >= this.animationParts.Length)
            {
                return;
            }

            this.AnimationChanged?.Invoke(this, $"{(this.UseBrackets ? "[" : "")}{this.animationParts[this.animationIndex]}{(this.UseBrackets ? "]" : "")}");

            this.animationIndex++;

            if (this.animationIndex == this.animationParts.Length)
            {
                this.animationIndex = 0;
            }
        }

        private void SetAnimationType()
        {
            this.animationParts = this.animationType switch
            {
                AnimationTypes.Slashes => this.slashes,
                AnimationTypes.Triangles => this.triangles,
                AnimationTypes.ClockCircle => this.clockCircles,
                AnimationTypes.ClockSquare => this.clockSquare,
                AnimationTypes.BlocksVertical => this.blocksVertical,
                AnimationTypes.BlocksHorizontal => this.blocksHorizontal,
                AnimationTypes.BlockChars => this.blockChars,
                _ => this.slashes,
            };
        }

        public void Start()
        {
            this.timer.Start();
        }

        public void Pause()
        {
            this.timer.Stop();
        }

        public void Stop()
        {
            this.animationIndex = 0;
            this.timer.Stop();
        }
    }
}
