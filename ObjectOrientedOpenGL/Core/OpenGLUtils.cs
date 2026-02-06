using System.Diagnostics;
using OpenTK.Graphics.OpenGL;
using ErrorCode = OpenTK.Graphics.OpenGL.ErrorCode;
using GL = OpenTK.Graphics.OpenGL.GL;

namespace ObjectOrientedOpenGL.Core;

public static class OpenGLUtils
{
    [Conditional("DEBUG")]
    public static void CheckError()
    {
        ErrorCode error;
        while ((error = GL.GetError()) != ErrorCode.NoError)
        {
            if (Debugger.IsAttached)
            {
                Debugger.Break();
            }

            Debug.Print($"Error: {error.ToString()}({(int)error})");
        }
    }
}