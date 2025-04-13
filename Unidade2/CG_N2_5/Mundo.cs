/*
 As constantes dos pré-processors estão nos arquivos ".csproj"
 desse projeto e da CG_Biblioteca.
*/

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;

    private char rotuloAtual = '?';
    private Dictionary<char, Objeto> grafoLista = [];
    private Objeto objetoSelecionado = null;
    private Transformacao4D matrizGrafo = new();
    private Circulo circuloMaior;
    private Circulo circuloMenor;
    private Retangulo box;
    private Ponto pontoCentral;
    private BBox bbox;

#if CG_Gizmo
    private readonly float[] _sruEixos =
    [
       0.0f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f,  0.0f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f,  0.0f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    ];
    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    // FPS
    private int frames = 0;
    private Stopwatch stopwatch = new();
#endif

    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;

    private bool mouseMovtoPrimeiro = true;
    private Ponto4D mouseMovtoUltimo;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
      : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloAtual); //padrão Singleton
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      Utilitario.Diretivas();
#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.5019f, 0.5019f, 0.6980f, 1.0f);

      #region Cores
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      #endregion

#if CG_Gizmo
      #region Eixos: SRU  
      _vertexBufferObject_sruEixos = GL.GenBuffer();
      GL.BindBuffer(BufferTarget.ArrayBuffer, _vertexBufferObject_sruEixos);
      GL.BufferData(BufferTarget.ArrayBuffer, _sruEixos.Length * sizeof(float), _sruEixos, BufferUsageHint.StaticDraw);
      _vertexArrayObject_sruEixos = GL.GenVertexArray();
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
      GL.EnableVertexAttribArray(0);
      #endregion

      stopwatch.Start();
#endif
      #region Joystick virtual  
            pontoCentral = new Ponto(mundo, ref rotuloAtual, new Ponto4D(0.2, 0.2)) {
                PrimitivaTamanho = 3,
                ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag")
            };

            Ponto4D ptoDeslocamento = new Ponto4D(0.2, 0.2);
            circuloMaior = new Circulo(mundo, ref rotuloAtual, 0.2, ptoDeslocamento) {
                ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag")
            };

            circuloMenor = new Circulo(mundo, ref rotuloAtual, 0.05, ptoDeslocamento) {
                ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag")
            };

            box = new Retangulo(mundo, ref rotuloAtual, circuloMaior.PontosId(9), circuloMaior.PontosId(45)) {
                PrimitivaTipo = PrimitiveType.LineLoop,
                ShaderObjeto = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag")
            };

            //adicionando um BBox no tamanho do quadrado
            List<Ponto4D> listaBBox = new List<Ponto4D> {circuloMaior.PontosId(9), circuloMaior.PontosId(45)};
            bbox = new BBox();
            bbox.Atualizar(new Transformacao4D(), listaBBox);
      #endregion
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      matrizGrafo.AtribuirIdentidade();
      mundo.Desenhar(matrizGrafo, objetoSelecionado);

#if CG_Gizmo
      Gizmo_Sru3D();

      frames++;
      if (stopwatch.ElapsedMilliseconds >= 1000)
      {
        Console.WriteLine($"FPS: {frames}");
        frames = 0; 
        stopwatch.Restart();
      }
#endif
      SwapBuffers();
    }

    protected override void OnUpdateFrame(FrameEventArgs e)
    {
      base.OnUpdateFrame(e);

      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyPressed(Keys.Escape))
        Close();

      #region Funções de apoio para o desenvolvimento. Não é do enunciado  
      if (estadoTeclado.IsKeyPressed(Keys.C) || estadoTeclado.IsKeyPressed(Keys.W))
      {
        moverPrincipal(0, 0.01);
      }

      if (estadoTeclado.IsKeyPressed(Keys.B) || estadoTeclado.IsKeyPressed(Keys.S))
      {
        moverPrincipal(0, -0.01);
      }

      if (estadoTeclado.IsKeyPressed(Keys.E) || estadoTeclado.IsKeyPressed(Keys.A))
      {
        moverPrincipal(-0.01, 0);
      }

      if (estadoTeclado.IsKeyPressed(Keys.D) || estadoTeclado.IsKeyPressed(Keys.D))
      {
        moverPrincipal(0.01, 0);
      }
      #endregion
      #endregion

      #region  Mouse
      int janelaLargura = ClientSize.X;
      int janelaAltura = ClientSize.Y;
      Ponto4D mousePonto = new(MousePosition.X, MousePosition.Y);
      Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

      if (estadoTeclado.IsKeyPressed(Keys.LeftShift))
      {
        if (mouseMovtoPrimeiro)
        {
          mouseMovtoUltimo = sruPonto;
          mouseMovtoPrimeiro = false;
        }
        else
        {
          var deltaX = sruPonto.X - mouseMovtoUltimo.X;
          var deltaY = sruPonto.Y - mouseMovtoUltimo.Y;
          mouseMovtoUltimo = sruPonto;

          objetoSelecionado.PontosAlterar(new Ponto4D(objetoSelecionado.PontosId(0).X + deltaX, objetoSelecionado.PontosId(0).Y + deltaY, 0), 0);
          objetoSelecionado.ObjetoAtualizar();
        }
      }
      if (estadoTeclado.IsKeyPressed(Keys.LeftShift))
      {
        objetoSelecionado.PontosAlterar(sruPonto, 0);
        objetoSelecionado.ObjetoAtualizar();
      }

      #endregion
    }

    private void moverPrincipal(double deslocX, double deslocY) {      
      Ponto4D aux = new Ponto4D(pontoCentral.PontosId(0).X + deslocX,  pontoCentral.PontosId(0).Y + deslocY);     

      // 0.04 é o quadrado da distância máxima permitida entre os centros dos dois círculos = 0,2 x 0,2
      if (circuloMaior.Bbox().Dentro(aux) && Matematica.DistanciaQuadrado(aux, new Ponto4D(0.2, 0.2)) < 0.04)
      {
          //atualiza o ponto principal
          pontoCentral.PontosId(0).X += deslocX;
          pontoCentral.PontosId(0).Y += deslocY;
          pontoCentral.Atualizar();

          //gera os pontos do circulo menor novamente
          Ponto4D ptoDeslocamento = new Ponto4D(deslocX, deslocY);
          circuloMenor.Atualizar(ptoDeslocamento);
          
          //valida se ainda está dentro do quadrado
          box.PrimitivaTipo = bbox.Dentro(pontoCentral.PontosId(0)) ?  PrimitiveType.LineLoop : PrimitiveType.Points;      
      }
  }

    protected override void OnResize(ResizeEventArgs e)
    {
      base.OnResize(e);

#if CG_DEBUG      
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif
      GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
    }

    protected override void OnUnload()
    {
      mundo.OnUnload();

      GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
      GL.BindVertexArray(0);
      GL.UseProgram(0);

#if CG_Gizmo
      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);
#endif

      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);

      base.OnUnload();
    }

    private void Gizmo_Sru3D()
    {
#if CG_Gizmo
#if CG_OpenGL
      var transform = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("transform", transform);
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("transform", transform);
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("transform", transform);
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#endif
#endif
    }

  }
}
