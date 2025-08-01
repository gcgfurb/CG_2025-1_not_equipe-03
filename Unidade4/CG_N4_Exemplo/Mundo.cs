﻿#define CG_DEBUG
#define CG_Gizmo
#define CG_OpenGL
// #define CG_OpenTK
// #define CG_DirectX      
// #define CG_Privado      

using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;
using OpenTK.Windowing.Desktop;
using System;
using OpenTK.Mathematics;
using System.Collections.Generic;
using System.Diagnostics;

//FIXME: padrão Singleton

namespace gcgcg
{
  public class Mundo : GameWindow
  {
    private static Objeto mundo = null;
    private char rotuloNovo = '?';
    private Objeto objetoSelecionado = null;
    private Objeto cuboMenor = null;
    private float orbita = 0.0f;
    private Vector2 posicaoMouseAnterior;
    private bool primeiroMovimentoMouse = true;
    private float sensibilidadeMouse = 0.2f;
    private Texture _diffuseMap;
    private Objeto _cuboLuz; // cubo emissor de luz fixo
    private readonly Vector3 _lightPos = new Vector3(1.2f, 1.0f, 2.0f);
    private Shader _lightingShader;
    private Shader _lampShader;


    private Shader shaderatual;
#if CG_Gizmo
    private readonly float[] _sruEixos =
    {
      -0.5f,  0.0f,  0.0f, /* X- */      0.5f,  0.0f,  0.0f, /* X+ */
       0.0f, -0.5f,  0.0f, /* Y- */      0.0f,  0.5f,  0.0f, /* Y+ */
       0.0f,  0.0f, -0.5f, /* Z- */      0.0f,  0.0f,  0.5f  /* Z+ */
    };

    private int _vertexBufferObject_sruEixos;
    private int _vertexArrayObject_sruEixos;

    // FPS
    private int frames = 0;
    private Stopwatch stopwatch = new();
#endif

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;
    private Shader _shaderTextura;
    private Shader _shaderBasicLighting;

    private Camera _camera;

    // Enum para controlar o shader ativo
    public enum ShaderModo
    {
      Textura,
      BasicLighting,
      Branca,
      Vermelha,
      Verde,
      Azul,
      Ciano,
      Magenta,
      Amarela
    }

    private ShaderModo modoShaderAtual = ShaderModo.Textura;

    public Mundo(GameWindowSettings gameWindowSettings, NativeWindowSettings nativeWindowSettings)
           : base(gameWindowSettings, nativeWindowSettings)
    {
      mundo ??= new Objeto(null, ref rotuloNovo); //padrão Singleton
    }

    public void SetModoShader(ShaderModo novoModo)
    {
      modoShaderAtual = novoModo;
      Console.WriteLine($"Shader alterado para: {modoShaderAtual}");
    }

    protected override void OnLoad()
    {
      base.OnLoad();

      CursorState = CursorState.Grabbed;

      Utilitario.Diretivas();

      _diffuseMap = Texture.LoadFromFile("resources/equipe.jpg");

#if CG_DEBUG
      Console.WriteLine("Tamanho interno da janela de desenho: " + ClientSize.X + "x" + ClientSize.Y);
#endif

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      GL.Enable(EnableCap.DepthTest);       // Ativar teste de profundidade
      GL.Enable(EnableCap.CullFace);         // Desenha os dois lados da face

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      _shaderTextura = new Shader("Shaders/shaderTextura.vert", "Shaders/shaderTextura.frag");
      _shaderBasicLighting = new Shader("Shaders/shader.vert", "Shaders/basicLighting.frag");

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

      stopwatch.Start();
      #endregion
#endif

      #region Objeto: Cubo
      objetoSelecionado = new Cubo(mundo, ref rotuloNovo);
      objetoSelecionado.shaderCor = _shaderTextura;
      cuboMenor = new Cubo(objetoSelecionado, ref rotuloNovo);
      cuboMenor.shaderCor = _shaderVermelha;
      // _cuboLuz = new Cubo(mundo, ref rotuloNovo);
      // _cuboLuz.shaderCor = _shaderBranca;
      // _cuboLuz.MatrizAtribuirIdentidade();
      // _cuboLuz.MatrizTranslacaoXYZ(5f, 5f, 5f);
      #endregion

      _camera = new Camera(Vector3.UnitZ * 5, ClientSize.X / (float)ClientSize.Y);
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

      _diffuseMap.Use(TextureUnit.Texture0);

      switch (modoShaderAtual)
      {
        case ShaderModo.Textura:
          _shaderTextura.Use();
          _shaderTextura.SetMatrix4("view", _camera.GetViewMatrix());
          _shaderTextura.SetMatrix4("projection", _camera.GetProjectionMatrix());
          _shaderTextura.SetInt("texture0", 0);
          break;

        case ShaderModo.BasicLighting:
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);


            _lightingShader.Use();

            _lightingShader.SetMatrix4("model", Matrix4.Identity);
            _lightingShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lightingShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            _lightingShader.SetVector3("objectColor", new Vector3(1.0f, 0.5f, 0.31f));
            _lightingShader.SetVector3("lightColor", new Vector3(1.0f, 1.0f, 1.0f));
            _lightingShader.SetVector3("lightPos", _lightPos);
            _lightingShader.SetVector3("viewPos", _camera.Position);

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);


            _lampShader.Use();

            Matrix4 lampMatrix = Matrix4.CreateScale(0.2f);
            lampMatrix = lampMatrix * Matrix4.CreateTranslation(_lightPos);

            _lampShader.SetMatrix4("model", lampMatrix);
            _lampShader.SetMatrix4("view", _camera.GetViewMatrix());
            _lampShader.SetMatrix4("projection", _camera.GetProjectionMatrix());

            GL.DrawArrays(PrimitiveType.Triangles, 0, 36);

            SwapBuffers();
          break;

        case ShaderModo.Branca:
          _shaderBranca.Use();
          break;

        case ShaderModo.Vermelha:
          _shaderVermelha.Use();
          break;

        case ShaderModo.Verde:
          _shaderVerde.Use();
          break;

        case ShaderModo.Azul:
          _shaderAzul.Use();
          break;

        case ShaderModo.Ciano:
          _shaderCiano.Use();
          break;

        case ShaderModo.Magenta:
          _shaderMagenta.Use();
          break;

        case ShaderModo.Amarela:
          _shaderAmarela.Use();
          break;

        default:
          _shaderTextura.Use();
          break;
      }
      _shaderTextura.Use();
      _shaderTextura.SetInt("texture0", 0);

      mundo.Desenhar(new Transformacao4D(), _camera);

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

      orbita += 100f * (float)e.Time;

      if (cuboMenor != null)
      {
        cuboMenor.MatrizAtribuirIdentidade();

        cuboMenor.MatrizEscalaXYZ(0.2, 0.2, 0.2);
        cuboMenor.MatrizTranslacaoXYZ(2.5, 0.0, 0.0);

        cuboMenor.TrocaEixoRotacao('y');
        cuboMenor.MatrizRotacao(orbita);
      }

      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyDown(Keys.Escape))
        Close();

      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        if (objetoSelecionado == null)
          objetoSelecionado = mundo;
        objetoSelecionado.shaderCor = _shaderBranca;
        objetoSelecionado = mundo.GrafocenaBuscaProximo(objetoSelecionado);
        objetoSelecionado.shaderCor = _shaderAmarela;
      }

      if (estadoTeclado.IsKeyPressed(Keys.G))
        mundo.GrafocenaImprimir("");
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
        Console.WriteLine(objetoSelecionado.ToString());
      if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.05, 0, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, -0.05, 0);
      if (estadoTeclado.IsKeyPressed(Keys.O) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0, 0.05);
      if (estadoTeclado.IsKeyPressed(Keys.L) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0, 0, -0.05);
      if (estadoTeclado.IsKeyPressed(Keys.PageUp) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(2, 2, 2);
      if (estadoTeclado.IsKeyPressed(Keys.PageDown) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.Home) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(0.5, 0.5, 0.5);
      if (estadoTeclado.IsKeyPressed(Keys.End) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(2, 2, 2);

      // Mapeamento para mudar shader via teclado:
      if (estadoTeclado.IsKeyPressed(Keys.D0))
      {
        SetModoShader(ShaderModo.Textura);
      }
      if (estadoTeclado.IsKeyPressed(Keys.D1))
      {
        _lightingShader = new Shader("Shaders/shader.vert", "Shaders/basicLighting.frag");
        _lampShader = new Shader("Shaders/shader.vert", "Shaders/shader.frag");
        SetModoShader(ShaderModo.BasicLighting);
      }
      if (estadoTeclado.IsKeyPressed(Keys.D2)) SetModoShader(ShaderModo.Branca);
      if (estadoTeclado.IsKeyPressed(Keys.D3)) SetModoShader(ShaderModo.Vermelha);
      if (estadoTeclado.IsKeyPressed(Keys.D4)) SetModoShader(ShaderModo.Verde);
      if (estadoTeclado.IsKeyPressed(Keys.D5)) SetModoShader(ShaderModo.Azul);
      if (estadoTeclado.IsKeyPressed(Keys.D6)) SetModoShader(ShaderModo.Ciano);
      if (estadoTeclado.IsKeyPressed(Keys.D7)) SetModoShader(ShaderModo.Magenta);
      if (estadoTeclado.IsKeyPressed(Keys.D8)) SetModoShader(ShaderModo.Amarela);

          const float cameraSpeed = 1.5f;
      if (estadoTeclado.IsKeyDown(Keys.Z))
        _camera.Position = Vector3.UnitZ * 5;
      if (estadoTeclado.IsKeyDown(Keys.W))
        _camera.Position += _camera.Front * cameraSpeed * (float)e.Time; // Forward
      if (estadoTeclado.IsKeyDown(Keys.S))
        _camera.Position -= _camera.Front * cameraSpeed * (float)e.Time; // Backwards
      if (estadoTeclado.IsKeyDown(Keys.A))
        _camera.Position -= _camera.Right * cameraSpeed * (float)e.Time; // Left
      if (estadoTeclado.IsKeyDown(Keys.D))
        _camera.Position += _camera.Right * cameraSpeed * (float)e.Time; // Right
      if (estadoTeclado.IsKeyDown(Keys.RightShift))
        _camera.Position += _camera.Up * cameraSpeed * (float)e.Time; // Up
      if (estadoTeclado.IsKeyDown(Keys.LeftShift))
        _camera.Position -= _camera.Up * cameraSpeed * (float)e.Time; // Down
      #endregion

      #region  Mouse
      Vector2 posicaoMouseAtual = MousePosition;

      if (primeiroMovimentoMouse)
      {
        posicaoMouseAnterior = posicaoMouseAtual;
        primeiroMovimentoMouse = false;
      }

      float deltaX = posicaoMouseAtual.X - posicaoMouseAnterior.X;
      float deltaY = posicaoMouseAtual.Y - posicaoMouseAnterior.Y;
      posicaoMouseAnterior = posicaoMouseAtual;

      deltaY = -deltaY;

      _camera.Yaw += deltaX * sensibilidadeMouse;
      _camera.Pitch += deltaY * sensibilidadeMouse;

      if (MouseState.IsButtonPressed(MouseButton.Left))
      {
        Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
        Console.WriteLine("__ Valores do Espaço de Tela");
        Console.WriteLine("Vector2 mousePosition: " + MousePosition);
        Console.WriteLine("Vector2i windowSize: " + ClientSize);
      }
      if (MouseState.IsButtonDown(MouseButton.Right) && objetoSelecionado != null)
      {
        Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");

        int janelaLargura = ClientSize.X;
        int janelaAltura = ClientSize.Y;
        Ponto4D mousePonto = new Ponto4D(MousePosition.X, MousePosition.Y);
        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(janelaLargura, janelaAltura, mousePonto);

        objetoSelecionado.PontosAlterar(sruPonto, 0);
      }
      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");
      }
      #endregion
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

      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

      base.OnUnload();
    }

#if CG_Gizmo
    private void Gizmo_Sru3D()
    {
#if CG_OpenGL && !CG_DirectX
      var model = Matrix4.Identity;
      GL.BindVertexArray(_vertexArrayObject_sruEixos);
      // EixoX
      _shaderVermelha.SetMatrix4("model", model);
      _shaderVermelha.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderVermelha.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderVermelha.Use();
      GL.DrawArrays(PrimitiveType.Lines, 0, 2);
      // EixoY
      _shaderVerde.SetMatrix4("model", model);
      _shaderVerde.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderVerde.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderVerde.Use();
      GL.DrawArrays(PrimitiveType.Lines, 2, 2);
      // EixoZ
      _shaderAzul.SetMatrix4("model", model);
      _shaderAzul.SetMatrix4("view", _camera.GetViewMatrix());
      _shaderAzul.SetMatrix4("projection", _camera.GetProjectionMatrix());
      _shaderAzul.Use();
      GL.DrawArrays(PrimitiveType.Lines, 4, 2);
#elif CG_DirectX && !CG_OpenGL
      Console.WriteLine(" .. Coloque aqui o seu código em DirectX");
#elif (CG_DirectX && CG_OpenGL) || (!CG_DirectX && !CG_OpenGL)
      Console.WriteLine(" .. ERRO de Render - escolha OpenGL ou DirectX !!");
#endif
    }
#endif    

  }
}
