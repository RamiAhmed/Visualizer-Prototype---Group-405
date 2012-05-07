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
    class CameraHandler
    {
        private Matrix _transform; 
        private Vector2 _pos; 
        private float _rotation, _zoom;
        private CameraState _currentState;

        private int lastZoom = 0, zoomIntervals = 10; // every nth second
        private float maxZoom = 4f, defaultZoom = 1f, zoomIncremental = 0.02f;

        public Vector2 defaultCameraPosition = new Vector2(Controller.TOTAL_WIDTH / 4, Controller.TOTAL_HEIGHT / 4);

        private enum CameraState { ZOOMING_IN, ZOOMING_OUT, IDLE };
            
        public CameraHandler()
        {
            this.Zoom = defaultZoom;
            this.Rotation = 0.0f;
            this.Position = defaultCameraPosition;
            this.CurrentState = CameraState.IDLE;
        }

        public void updateCamera(GameTime time, Vector2 heroPosition)
        {
            int currentSeconds = (int)time.TotalGameTime.TotalSeconds;
            if (this.CurrentState == CameraState.IDLE)
            {
                if (currentSeconds - lastZoom > zoomIntervals)
                {
                    Console.WriteLine("Zooming in");
                    lastZoom = currentSeconds;
                    //this.Move(heroPosition);
                    this.Zoom = defaultZoom;
                    this.CurrentState = CameraState.ZOOMING_IN;
                }
            }
            else if (this.CurrentState == CameraState.ZOOMING_IN) 
            {
                if (this.Zoom <= maxZoom)
                {
                    this.Move(this.Position.X, this.Position.Y + 0.75f);
                    this.Zoom += zoomIncremental;
                }
                else
                {
                    Console.WriteLine("Zooming out");
                    //this.Position = defaultCameraPosition;
                    //this.Zoom = defaultZoom;
                    this.CurrentState = CameraState.ZOOMING_OUT;
                }
            }
            else if (this.CurrentState == CameraState.ZOOMING_OUT)
            {
                if (this.Zoom > defaultZoom)
                {
                    this.Move(this.Position.X, this.Position.Y - 0.75f);
                    this.Zoom -= zoomIncremental;
                }
                else
                {
                    Console.WriteLine("Done zooming, back at default");
                    this.CurrentState = CameraState.IDLE;
                }
            }
        }



        public void Move(Vector2 position)
        {
            this.Position = position;
        }
        public void Move(int x, int y)
        {
            this.Position = new Vector2(x, y);
        }
        public void Move(float x, float y)
        {
            this.Position = new Vector2(x, y);
        }
                 

        public Matrix getTransformation(GraphicsDevice graphicsDevice)
        {
            _transform =  
                         Matrix.CreateTranslation(new Vector3(-this.Position.X, -this.Position.Y, 0)) *
                         Matrix.CreateRotationZ(MathHelper.ToRadians(this.Rotation)) *
                         Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 1f)) *
                         Matrix.CreateTranslation(new Vector3(graphicsDevice.Viewport.Width * 0.5f - this.Position.X * this.Zoom,
                                                              graphicsDevice.Viewport.Height * 0.5f - this.Position.Y * this.Zoom, 0));
            return _transform;
        }

        public float Zoom
        {
            get { return _zoom; }
            set { _zoom = value; if (_zoom < 0.1f) _zoom = 0.1f; } // Negative zoom will flip image
        }

        public float Rotation
        {
            get { return _rotation; }
            set { _rotation = value; }
        }

        public Vector2 Position
        {
            get { return _pos; }
            set { _pos = value; }
        }

        private CameraState CurrentState
        {
            get { return _currentState; }
            set { _currentState = value; }
        }

    }
}
