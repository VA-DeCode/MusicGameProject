using Silk.NET.Maths;
using Silk.NET.OpenGL;

namespace MusicGameProject.Core
{
    public class ShaderProgram : IDisposable
    {
        private uint _handle;
        private GL _gl;

        public ShaderProgram(GL gl, string vPath, string fPath)
        {
            _gl = gl;
            uint vertex = LoadShader(ShaderType.VertexShader, File.ReadAllText(vPath));
            uint fragment = LoadShader(ShaderType.FragmentShader, File.ReadAllText(fPath));
            _handle = _gl.CreateProgram();
            _gl.AttachShader(_handle, vertex);
            _gl.AttachShader(_handle, fragment);
            _gl.LinkProgram(_handle);
            _gl.DeleteShader(vertex);
            _gl.DeleteShader(fragment);
        }

        public void Use() => _gl.UseProgram(_handle);
        public unsafe void SetUniform(string name, Matrix4X4<float> value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location != -1)
                _gl.UniformMatrix4(location, 1, false, (float*)&value);
        }

        public void SetUniform(string name, Vector4D<float> value)
        {
            int location = _gl.GetUniformLocation(_handle, name);
            if (location != -1)
                _gl.Uniform4(location, value.X, value.Y, value.Z, value.W);
        }

        private uint LoadShader(ShaderType type, string src)
        {
            uint handle = _gl.CreateShader(type);
            _gl.ShaderSource(handle, src);
            _gl.CompileShader(handle);
            return handle;
        }

        public void Dispose() => _gl.DeleteProgram(_handle);
    }
}