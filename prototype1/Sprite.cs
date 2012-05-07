using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace prototype1
{
    class Sprite
    {
        private Texture2D _texture;
        private Vector2 _position;
        private float _speed, _layer, _scaleFactor;
        private bool _active;
        private Color _color;
        private int _width, _height;

        public Sprite()
        {
            this.Color = Color.White;
            this.Speed = 1f;
            this.Active = false;
            this.Width = 0;
            this.Height = 0;
        }

        public void CheckIsActive()
        {
            if (this.Active)
            {
                if (this.Width <= 0 && this.Height <= 0 && this.Texture != null)
                {
                    this.Width = this.Texture.Width;
                    this.Height = this.Texture.Height;
                }

                if ((this.Position.X < -this.Width  || this.Position.X > Controller.TOTAL_WIDTH + this.Width) ||
                    (this.Position.Y < -this.Height || this.Position.Y > Controller.TOTAL_HEIGHT + this.Height))
                {
                    this.Position = new Vector2(-this.Texture.Width, -this.Texture.Height);
                    this.Active = false;
                }
            }
        }

        public void Move(Vector2 pos)
        {
            this.Position = pos;
        }

        public void Move(int x, int y)
        {
            this.Position = new Vector2(x, y);
        }

        public void Move(float x, float y)
        {
            this.Position = new Vector2(x, y);
        }

        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        public bool Active
        {
            get { return _active; }
            set { _active = value; }
        }

        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }

        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public int Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public float LayerDepth
        {
            get { return _layer; }
            set { _layer = value; }
        }

        public float ScaleFactor
        {
            get { return _scaleFactor; }
            set { _scaleFactor = value; }
        }
    }
}
