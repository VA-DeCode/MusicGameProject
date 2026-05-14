using Silk.NET.Maths;
using Silk.NET.OpenGL;
using System;

namespace MusicGameProject.Core
{
    public class ShapeRenderer : IDisposable
    {
        private GL _gl;
        private uint _vbo;
        private uint _vao;
        private ShaderProgram _shader;
        private Matrix4X4<float> _currentProjection;

        public ShapeRenderer(GL gl)
        {
            _gl = gl;
            float[] vertices = { 0f, 0f, 1f, 0f, 1f, 1f, 1f, 1f, 0f, 1f, 0f, 0f };

            _vbo = _gl.GenBuffer();
            _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
            _gl.BufferData<float>(BufferTargetARB.ArrayBuffer, (ReadOnlySpan<float>)vertices.AsSpan(), BufferUsageARB.StaticDraw);

            _vao = _gl.GenVertexArray();
            _gl.BindVertexArray(_vao);
            _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 2 * sizeof(float), 0);
            _gl.EnableVertexAttribArray(0);

            _shader = new ShaderProgram(_gl, Config.Paths.VertexShader, Config.Paths.FragmentShader);
        }

        public void BeginLayer(Camera camera, Layer layer)
        {
            _shader.Use();
            _gl.BindVertexArray(_vao);

            if (layer == Layer.UI)
                _currentProjection = camera.GetUIProjectionMatrix();
            else if (layer == Layer.Background)
                _currentProjection = camera.GetProjectionMatrix(Config.Graphics.BackgroundParallax);
            else
                _currentProjection = camera.GetProjectionMatrix(Config.Graphics.DefaultParallax);
        }

        public void Submit(Vector2D<float> pos, Vector2D<float> size, Vector4D<float> color)
        {
            var model = Matrix4X4.CreateScale(size.X, size.Y, 1f) *
                        Matrix4X4.CreateTranslation(pos.X, pos.Y, 0f);

            _shader.SetUniform("uProjection", model * _currentProjection);
            _shader.SetUniform("uColor", color);

            _gl.DrawArrays(PrimitiveType.Triangles, 0, 6);
        }

        public void EndLayer()
        {
            // Здесь в будущем будет вызов glDrawElements для всего накопленного буфера
            _gl.BindVertexArray(0);
        }

        public void Dispose()
        {
            _gl.DeleteBuffer(_vbo);
            _gl.DeleteVertexArray(_vao);
            _shader.Dispose();
        }
    }
}