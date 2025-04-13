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

    private Spline spline;

    private List<Ponto4D> listaPontosControle;

    private List<Ponto> objetosPontosVisuais;

    private List<SegReta> objetosPoliedroControle;

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
    private Shader _shaderBranca;
    private Shader _shaderAmarelo;
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
      _shaderAmarelo = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
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
      #region Objeto: Spline
        listaPontosControle = new List<Ponto4D>() {
          new Ponto4D(-0.8, -0.5, 0),
          new Ponto4D(-0.5, 0.5, 0),
          new Ponto4D(0.5, 0.5, 0),
          new Ponto4D(0.8, -0.5, 0)
        };
          
        spline = new Spline(mundo, ref rotuloAtual, listaPontosControle);

        objetosPontosVisuais = new List<Ponto>();

        foreach (var p4d in listaPontosControle)
        {
          objetosPontosVisuais.Add(new Ponto(mundo, ref rotuloAtual, p4d));
        }

        objetosPoliedroControle = new List<SegReta>();

        for (int i = 0; i < listaPontosControle.Count - 1; i++)
        {
          var segObj = new SegReta(mundo, ref rotuloAtual, listaPontosControle[i], listaPontosControle[i + 1]);
          objetosPoliedroControle.Add(segObj);
        }

        foreach (var item in listaPontosControle)
        {
          AtualizarVisualPoliedro(listaPontosControle.IndexOf(item));
          AtualizarVisualPonto(listaPontosControle.IndexOf(item));
        }
      #endregion
    }

    protected override void OnRenderFrame(FrameEventArgs e)
    {
      base.OnRenderFrame(e);

      GL.Clear(ClearBufferMask.ColorBufferBit);

      matrizGrafo.AtribuirIdentidade();

      foreach (var segReta  in objetosPoliedroControle)
      {
        segReta.ShaderObjeto = _shaderCiano;
        segReta.Desenhar(matrizGrafo, null);
      }

      spline.ShaderObjeto = _shaderAmarelo;
      spline.Desenhar(matrizGrafo, null);

      int indiceSelecionado = spline.SelectedControlPointIndex;

      for (int i = 0; i < objetosPontosVisuais.Count; i++)
      {
          if (i == indiceSelecionado)
          {
            objetosPontosVisuais[i].ShaderObjeto = _shaderVermelha;
          }
          else
          {
            objetosPontosVisuais[i].ShaderObjeto = _shaderBranca;
          }
          objetosPontosVisuais[i].Desenhar(matrizGrafo, null);
      }

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
      
      int indiceSelecionadoSpline = spline.SelectedControlPointIndex;
      double incrementoMovimento = 0.1; // Ajuste a velocidade do movimento

      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
          spline.TrocarPontoDeControleSelecionado();
      }
      else if (estadoTeclado.IsKeyPressed(Keys.C)) // Mover para Cima
      {
          spline.MoverPontoControle(indiceSelecionadoSpline, 0, incrementoMovimento);
          AtualizarVisualPonto(indiceSelecionadoSpline); // ATUALIZA O PONTO VISUAL
          AtualizarVisualPoliedro(indiceSelecionadoSpline); // ATUALIZA AS LINHAS VISUAIS
      }
      else if (estadoTeclado.IsKeyPressed(Keys.B)) // Mover para Baixo
      {
          spline.MoverPontoControle(indiceSelecionadoSpline, 0, -incrementoMovimento);
          AtualizarVisualPonto(indiceSelecionadoSpline);
          AtualizarVisualPoliedro(indiceSelecionadoSpline);
      }
      else if (estadoTeclado.IsKeyPressed(Keys.E)) // Mover para Esquerda
      {
          spline.MoverPontoControle(indiceSelecionadoSpline, -incrementoMovimento, 0);
          AtualizarVisualPonto(indiceSelecionadoSpline);
          AtualizarVisualPoliedro(indiceSelecionadoSpline);
      }
      else if (estadoTeclado.IsKeyPressed(Keys.D)) // Mover para Direita
      {
          spline.MoverPontoControle(indiceSelecionadoSpline, incrementoMovimento, 0);
          AtualizarVisualPonto(indiceSelecionadoSpline);
          AtualizarVisualPoliedro(indiceSelecionadoSpline);
      }
      // Aumentar/Diminuir pontos da CURVA (não de controle)
      else if (estadoTeclado.IsKeyPressed(Keys.KeyPadAdd))
      {
          spline.MudarNumeroPontosCurva(10);
      }
      else if (estadoTeclado.IsKeyPressed(Keys.Comma))
      {
          spline.MudarNumeroPontosCurva(-10);
      }

      if (estadoTeclado.IsKeyPressed(Keys.Escape))
        Close();

      #region Funções de apoio para o desenvolvimento. Não é do enunciado  
      if (estadoTeclado.IsKeyPressed(Keys.Space))
        objetoSelecionado = Grafocena.GrafoCenaProximo(mundo, objetoSelecionado, grafoLista);

      if (estadoTeclado.IsKeyPressed(Keys.F))
        Grafocena.GrafoCenaImprimir(mundo, grafoLista);
      if (estadoTeclado.IsKeyPressed(Keys.T))
      {
        if (objetoSelecionado != null)
          Console.WriteLine(objetoSelecionado);
        else
          Console.WriteLine("objetoSelecionado: MUNDO \n__________________________________\n");
      }

      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
      {
        if (objetoSelecionado.PontosListaTamanho > 0)
        {
          objetoSelecionado.PontosAlterar(new Ponto4D(objetoSelecionado.PontosId(0).X + 0.005, objetoSelecionado.PontosId(0).Y, 0), 0);
          objetoSelecionado.ObjetoAtualizar();
        }
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

    private void AtualizarVisualPonto(int index)
    {
        Ponto4D pontoAtualizado = listaPontosControle[index];

        if (index >= 0 && index < objetosPontosVisuais.Count)
        {
            objetosPontosVisuais[index].AtualizarPosicao(pontoAtualizado);
        }
    }

    private void AtualizarVisualPoliedro(int indexPontoMovido)
    {
        Ponto4D pontoAtualizado = listaPontosControle[indexPontoMovido];

        // Atualiza a linha ANTERIOR
        if (indexPontoMovido > 0)
        {
            int indexRetaAnterior = indexPontoMovido - 1;
            if (indexRetaAnterior >= 0 && indexRetaAnterior < objetosPoliedroControle.Count)
            {
                // Pega o ponto inicial da reta anterior (que não mudou)
                Ponto4D pontoInicialReta = listaPontosControle[indexPontoMovido - 1];
                // USA O NOVO MÉTODO DA CLASSE SegReta
                objetosPoliedroControle[indexRetaAnterior].AtualizarPontos(pontoInicialReta, pontoAtualizado);
            }
        }

        // Atualiza a linha SEGUINTE
        if (indexPontoMovido < listaPontosControle.Count - 1)
        {
            int indexRetaSeguinte = indexPontoMovido;
            if (indexRetaSeguinte >= 0 && indexRetaSeguinte < objetosPoliedroControle.Count)
            {
                // Pega o ponto final da reta seguinte (que não mudou)
                Ponto4D pontoFinalReta = listaPontosControle[indexPontoMovido + 1];
                // USA O NOVO MÉTODO DA CLASSE SegReta
                objetosPoliedroControle[indexRetaSeguinte].AtualizarPontos(pontoAtualizado, pontoFinalReta);
            }
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
