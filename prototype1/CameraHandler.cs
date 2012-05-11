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

        private float viewportWidth = 0f,
                      viewportHeight = 0f;

        private int lastZoom = 0, zoomIntervals = 25; // every nth second
        private float maxZoom = 4f, defaultZoom, zoomIncremental = 0.02f;
        private float yZoom = 30f, xZoom = 13.75f;
        
        public Vector2 defaultCameraPosition = new Vector2(Controller.TOTAL_WIDTH / 4, Controller.TOTAL_HEIGHT / 4);

        private enum CameraState { ZOOMING_IN, ZOOMING_OUT, IDLE };

        private bool debug = false;
            
        public CameraHandler()
        {
            if (Controller.IS_FULL_SCREEN)
            {
                defaultZoom = 1.3f;
            }
            else
            {
                defaultZoom = 1f;
            }
            this.Zoom = defaultZoom;
            this.Rotation = 0.0f;
            this.Position = defaultCameraPosition;  
                                                  
            this.CurrentState = CameraState.IDLE;
        }

        public void updateCamera(GameTime time, Vector2 heroPosition)
        {
            //this.Move(viewportWidth * 0.25f - Mouse.GetState().X, viewportHeight * 0.25f - Mouse.GetState().Y);
            //Vector2 mouseVec = new Vector2(Mouse.GetState().X, Mouse.GetState().Y);
            //Console.WriteLine(mouseVec.ToString());

            if (GameStateHandler.CurrentState == GameState.STARTING)
            {
                //Vector2 lookPos = new Vector2(350, 385);
                //moveCameraTo(lookPos);
                //this.Zoom = 1f;             
            }
            else if (GameStateHandler.CurrentState == GameState.RUNNING)
            {
                int currentSeconds = (int)time.TotalGameTime.TotalSeconds;
                if (this.CurrentState == CameraState.IDLE)
                {
                    if (currentSeconds - lastZoom > zoomIntervals)
                    {
                        if (debug)
                        {
                            Console.WriteLine("Zooming in");
                        }
                        lastZoom = currentSeconds;
                        this.Zoom = defaultZoom;
                        this.CurrentState = CameraState.ZOOMING_IN;

                    }
                }
                else if (this.CurrentState == CameraState.ZOOMING_IN)
                {
                    if (this.Zoom < maxZoom)
                    {
                        this.Move(this.Position.X - xZoom * zoomIncremental, this.Position.Y + yZoom * zoomIncremental);
                        this.Zoom += zoomIncremental;
                    }
                    else
                    {
                        if (debug)
                        {
                            Console.WriteLine("Zooming out");
                        }
                        this.CurrentState = CameraState.ZOOMING_OUT;
                    }
                }
                else if (this.CurrentState == CameraState.ZOOMING_OUT)
                {
                    if (this.Zoom > defaultZoom)
                    {
                        this.Move(this.Position.X + xZoom * zoomIncremental, this.Position.Y - yZoom * zoomIncremental);
                        this.Zoom -= zoomIncremental;
                    }
                    else
                    {
                        //this.Move(defaultCameraPosition);
                        if (debug)
                        {
                            Console.WriteLine("Done zooming, back at default");
                        }
                        this.CurrentState = CameraState.IDLE;
                    }
                }
            }
        }

        public void moveCameraTo(Vector2 position)
        {
            position = new Vector2(Math.Abs(position.X - (viewportWidth * 0.5f)), 
                                   Math.Abs(position.Y - (viewportHeight * 0.5f)));

            Vector2 movementVector = Vector2.Zero;
            float tolerance = 10f,
                  incremental = 0.5f;

            if (position.X - this.Position.X > tolerance)
            {
                movementVector.X += incremental;
            }
            if (position.X - this.Position.X < -tolerance)
            {
                movementVector.X -= incremental;
            }

            if (position.Y - this.Position.Y > -tolerance)
            {
                movementVector.Y += incremental;
            }
            if (position.Y - this.Position.Y < tolerance)
            {
                movementVector.Y -= incremental;
            }

            this.Position += movementVector; 
        }
        /*
        public void Move(Vector2 normalizedPos)
        {
            this.Position += normalizedPos;
        }
        */

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
            if (viewportHeight == 0) 
            {
                viewportHeight = graphicsDevice.Viewport.Height;
            }
            if (viewportWidth == 0)
            {
                viewportWidth = graphicsDevice.Viewport.Width;
            }

            _transform =  
                         Matrix.CreateTranslation(new Vector3(-this.Position.X, -this.Position.Y, 0)) *
                        // Matrix.CreateRotationZ(MathHelper.ToRadians(this.Rotation)) *
                         Matrix.CreateScale(new Vector3(this.Zoom, this.Zoom, 1f)) *
                         Matrix.CreateTranslation(new Vector3(viewportWidth * 0.5f - this.Position.X * this.Zoom,
                                                              viewportHeight * 0.5f - this.Position.Y * this.Zoom, 0));
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
