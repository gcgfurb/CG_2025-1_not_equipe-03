﻿/*
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
    private Objeto objetoNovo = null;
    private Transformacao4D matrizGrafo = new();

    //CRIADOS POR NOS
    private Ponto4D sruPonto = null;
    private List<Poligono> poligonos = [];

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

    private Shader _shaderBranca;
    private Shader _shaderVermelha;
    private Shader _shaderVerde;
    private Shader _shaderAzul;
    private Shader _shaderCiano;
    private Shader _shaderMagenta;
    private Shader _shaderAmarela;

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

      GL.ClearColor(0.0f, 0.0f, 0.0f, 1.0f);

      #region Cores
      _shaderBranca = new Shader("Shaders/shader.vert", "Shaders/shaderBranca.frag");
      _shaderVermelha = new Shader("Shaders/shader.vert", "Shaders/shaderVermelha.frag");
      _shaderVerde = new Shader("Shaders/shader.vert", "Shaders/shaderVerde.frag");
      _shaderAzul = new Shader("Shaders/shader.vert", "Shaders/shaderAzul.frag");
      _shaderCiano = new Shader("Shaders/shader.vert", "Shaders/shaderCiano.frag");
      _shaderMagenta = new Shader("Shaders/shader.vert", "Shaders/shaderMagenta.frag");
      _shaderAmarela = new Shader("Shaders/shader.vert", "Shaders/shaderAmarela.frag");
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

      #region Objeto: polígono qualquer, só para testes e ajudar no desenvolvimento  
      List<Ponto4D> pontosPoligonoBandeiraA =
      [
        new Ponto4D(0.25, 0.25),
        new Ponto4D(0.75, 0.25),
        new Ponto4D(0.75, 0.75),
        new Ponto4D(0.50, 0.50),
        new Ponto4D(0.25, 0.75),
      ];
      objetoSelecionado = new Poligono(mundo, ref rotuloAtual, pontosPoligonoBandeiraA);
      poligonos.Add((Poligono)objetoSelecionado);
      #endregion

      List<Ponto4D> teste =
      [
        new Ponto4D(0.0, 0.0),
        new Ponto4D(0.2, 0.0),
        new Ponto4D(0.2, 0.2),
        new Ponto4D(0.0, 0.2),
      ];
      objetoSelecionado = new Poligono(objetoSelecionado, ref rotuloAtual, teste);
      poligonos.Add((Poligono)objetoSelecionado);

      #region Objeto: polígono qualquer, só para testes e ajudar no desenvolvimento  
      List<Ponto4D> pontosPoligonoBandeiraB =
      [
        new Ponto4D(-0.25, -0.25),
        new Ponto4D(-0.75, -0.25),
        new Ponto4D(-0.75, -0.75),
        new Ponto4D(-0.50, -0.50),
        new Ponto4D(-0.25, -0.75),
      ];
      objetoSelecionado = new Poligono(mundo, ref rotuloAtual, pontosPoligonoBandeiraB);
      poligonos.Add((Poligono)objetoSelecionado);
      #endregion

      // objetoSelecionado = null;
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

      // ☞ 396c2670-8ce0-4aff-86da-0f58cd8dcfdc   TODO: forma otimizada para teclado.
      #region Teclado
      var estadoTeclado = KeyboardState;
      if (estadoTeclado.IsKeyPressed(Keys.Escape))
        Close();

      #region Funções de apoio para o desenvolvimento. Não é do enunciado  
      if (estadoTeclado.IsKeyPressed(Keys.Space))
      {
        objetoSelecionado = Grafocena.GrafoCenaProximo(mundo, objetoSelecionado, grafoLista);
        if (objetoSelecionado != null)
          objetoSelecionado.ObjetoAtualizar();
      }

      if (estadoTeclado.IsKeyPressed(Keys.F))
        Grafocena.GrafoCenaImprimir(mundo, grafoLista);
      if (estadoTeclado.IsKeyPressed(Keys.T))
      {
        if (objetoSelecionado != null)
          Console.WriteLine(objetoSelecionado);
        else
          Console.WriteLine("objetoSelecionado: MUNDO \n__________________________________\n");
      }

      if (estadoTeclado.IsKeyPressed(Keys.M) && objetoSelecionado != null)
        objetoSelecionado.MatrizImprimir();
      if (estadoTeclado.IsKeyPressed(Keys.I) && objetoSelecionado != null)
        objetoSelecionado.MatrizAtribuirIdentidade();
      if (estadoTeclado.IsKeyPressed(Keys.N) && objetoSelecionado != null)
        objetoSelecionado = null;
      #endregion

      // ## 2. Estrutura de dados: polígono
      // Quando pressionar a tecla Enter finaliza o desenho do novo polígono.  
      if (estadoTeclado.IsKeyPressed(Keys.Enter))
      {
        Console.WriteLine("## 2. Estrutura de dados: polígono - Enter");
        objetoSelecionado = objetoNovo;
        poligonos.Add((Poligono)objetoSelecionado);
        objetoNovo = null;
      }

      // ## 3. Estrutura de dados: polígono
      // Utilize a tecla D para remover o polígono selecionado.  
      if (estadoTeclado.IsKeyPressed(Keys.D) && objetoSelecionado != null)
      {
        Console.WriteLine("## 3. Estrutura de dados: polígono - Tecla D");
        objetoSelecionado.ObjetoRemover();
      }

      // ## 4. Estrutura de dados: vértices mover
      // Utilize a posição do mouse junto com a tecla V para mover vértice mais próximo do polígono selecionado.  
      if (estadoTeclado.IsKeyDown(Keys.V) && objetoSelecionado != null)
      {
        Console.WriteLine("## 4. Estrutura de dados: vértices mover - Tecla V");
        sruPonto = Utilitario.NDC_TelaSRU(ClientSize.X, ClientSize.Y, new Ponto4D(MousePosition.X, MousePosition.Y));
        sruPonto = objetoSelecionado.MatrizGlobalInversa(sruPonto);
        objetoSelecionado.PontosAlterar(sruPonto, objetoSelecionado.PontoMaisPerto(sruPonto, remover: false));
      }

      // ## 5. Estrutura de dados: vértices remover
      // Utilize a tecla E para remover o vértice do polígono selecionado mais próximo do ponto do mouse.  
      if (estadoTeclado.IsKeyPressed(Keys.E) && objetoSelecionado != null)
      {
        Console.WriteLine("## 5. Estrutura de dados: vértices remover - Tecla E");
        sruPonto = Utilitario.NDC_TelaSRU(ClientSize.X, ClientSize.Y, new Ponto4D(MousePosition.X, MousePosition.Y));
        sruPonto = objetoSelecionado.MatrizGlobalInversa(sruPonto);
        objetoSelecionado.PontoMaisPerto(sruPonto);
      }

      // ## 7. Interação: desenho
      // Utilize a tecla P para poder mudar o polígono selecionado para aberto ou fechado.  
      if (estadoTeclado.IsKeyPressed(Keys.P) && objetoSelecionado != null)
      {
        Console.WriteLine("## 7. Interação: desenho - Tecla P");
        if (objetoSelecionado.PrimitivaTipo == PrimitiveType.LineLoop)
        {
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineStrip;
          Console.WriteLine($"Polígono '{objetoSelecionado.Rotulo}' alterado para Aberto (LineStrip).");
        }
        else
        {
          objetoSelecionado.PrimitivaTipo = PrimitiveType.LineLoop;
          Console.WriteLine($"Polígono '{objetoSelecionado.Rotulo}' alterado para Fechado (LineLoop).");
        }
      }

      // ## 8. Interação: cores
      // Utilize o teclado (teclas R=vermelho,G=verde,B=azul) para trocar as cores dos polígonos selecionado.  
      if (estadoTeclado.IsKeyPressed(Keys.R) && objetoSelecionado != null)  // R=vermelho
        objetoSelecionado.ShaderObjeto = _shaderVermelha;
      if (estadoTeclado.IsKeyPressed(Keys.G) && objetoSelecionado != null)  // G=verde
        objetoSelecionado.ShaderObjeto = _shaderVerde;
      if (estadoTeclado.IsKeyPressed(Keys.B) && objetoSelecionado != null)  // B=azul
        objetoSelecionado.ShaderObjeto = _shaderAzul;

      // ## 10. Transformações Geométricas: translação
      // Utilizando as teclas das setas direcionais (cima/baixo,direita,esquerda) movimente o polígono selecionado.  
      if (estadoTeclado.IsKeyPressed(Keys.Left) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(-0.1,0,0);
      if (estadoTeclado.IsKeyPressed(Keys.Right) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0.1,0,0);
      if (estadoTeclado.IsKeyPressed(Keys.Up) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0,0.1,0);
      if (estadoTeclado.IsKeyPressed(Keys.Down) && objetoSelecionado != null)
        objetoSelecionado.MatrizTranslacaoXYZ(0,-0.1,0);
      // ## 11. Transformações Geométricas: escala
      // Utilizando as teclas PageUp/PageDown redimensione o polígono selecionado em relação ao SRU.  [TODO: testar]
      if (estadoTeclado.IsKeyPressed(Keys.F1) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(2,2,2);
      if (estadoTeclado.IsKeyPressed(Keys.F2) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZ(0.5,0.5,0.5);
      // Utilizando as teclas Home/End redimensione o polígono selecionado em relação ao centro da sua BBox.  [TODO: testar]
      if (estadoTeclado.IsKeyPressed(Keys.F3) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(2,2,2);
      if (estadoTeclado.IsKeyPressed(Keys.F4) && objetoSelecionado != null)
        objetoSelecionado.MatrizEscalaXYZBBox(0.5,0.5,0.5);
      // ## 12. Transformações Geométricas: rotação
      // Utilizando as teclas numéricas 1 e 2 gire o polígono selecionado em relação ao SRU.
      if (estadoTeclado.IsKeyPressed(Keys.D1) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(10);
      if (estadoTeclado.IsKeyPressed(Keys.D2) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacao(-10);
      // Utilizando as teclas numéricas 3 e 4 gire o polígono selecionado em relação ao centro da sua BBox.
      if (estadoTeclado.IsKeyPressed(Keys.D3) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(10);
      if (estadoTeclado.IsKeyPressed(Keys.D4) && objetoSelecionado != null)
        objetoSelecionado.MatrizRotacaoZBBox(-10);
      #endregion

      #region  Mouse

      // ## 2. Estrutura de dados: polígono
      // Utilize o mouse para clicar na tela com botão direito e poder desenhar um novo polígono.  
      if (MouseState.IsButtonPressed(MouseButton.Right))
      {
        Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Right)");
        sruPonto = Utilitario.NDC_TelaSRU(ClientSize.X, ClientSize.Y, new Ponto4D(MousePosition.X, MousePosition.Y));

        if (objetoNovo == null)
        {
          objetoNovo = objetoSelecionado == null ? new Poligono(mundo, ref rotuloAtual, new List<Ponto4D>()) : new Poligono(objetoSelecionado, ref rotuloAtual, new List<Ponto4D>());
          objetoNovo.PontosAdicionar(sruPonto);
          objetoNovo.PontosAdicionar(sruPonto);
        }
      }
      if (MouseState.IsButtonReleased(MouseButton.Right))
      {
        Console.WriteLine("MouseState.IsButtonReleased(MouseButton.Right)");

        if (objetoNovo != null)
        {
          objetoNovo.PontosAdicionar(sruPonto);
        }
      }

      // ## 6. Visualização: rastro
      // Exiba o “rasto” ao desenhar os segmentos do polígono.  
      if (MouseState.IsButtonDown(MouseButton.Right))
      {
        Console.WriteLine("MouseState.IsButtonDown(MouseButton.Right)");
        sruPonto = Utilitario.NDC_TelaSRU(ClientSize.X, ClientSize.Y, new Ponto4D(MousePosition.X, MousePosition.Y));

        sruPonto = objetoNovo.MatrizGlobalInversa(sruPonto);
        objetoNovo.PontosAlterar(sruPonto, 0);
      }

      // ## 9. Interação: BBox
      // Utilize o mouse para clicar na tela com botão esquerdo para selecionar o polígono testando primeiro se o ponto do mouse está dentro da BBox do polígono e depois usando o algoritmo Scan Line.  
      // Caso o polígono seja selecionado se deve exibir a sua BBbox, caso contrário a variável objetoSelecionado deve ser "null", e assim nenhum contorno de BBox deve ser exibido.  
      if (MouseState.IsButtonPressed(MouseButton.Left) && objetoNovo == null)
      {
        Console.WriteLine("MouseState.IsButtonPressed(MouseButton.Left)");
        Console.WriteLine("__ Valores do Espaço de Tela");
        Console.WriteLine("Vector2i windowSize: " + ClientSize);
        Console.WriteLine("Vector2 mousePosition: " + MousePosition);

        Ponto4D sruPonto = Utilitario.NDC_TelaSRU(ClientSize.X, ClientSize.Y, new Ponto4D(MousePosition.X, MousePosition.Y));
        Console.WriteLine("mousePosition (SRU): " + sruPonto);

        objetoSelecionado = null;
        foreach (var poligo in poligonos)
        {
          if (poligo.Bbox().Dentro(sruPonto))
          {
            objetoSelecionado = poligo;
            if (poligo.ScanLine(sruPonto, ref objetoSelecionado))
            {
              break;
            }
            objetoSelecionado = null;
          }
        }

        if (objetoSelecionado != null)
        {
          Console.WriteLine("Objeto selecionado com sucesso!");
          objetoSelecionado.Bbox().Desenhar();
        }
        else
        {
          Console.WriteLine("Nenhum objeto selecionado.");
        }
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

#if CG_Gizmo
      GL.DeleteBuffer(_vertexBufferObject_sruEixos);
      GL.DeleteVertexArray(_vertexArrayObject_sruEixos);
#endif

      GL.DeleteProgram(_shaderBranca.Handle);
      GL.DeleteProgram(_shaderVermelha.Handle);
      GL.DeleteProgram(_shaderVerde.Handle);
      GL.DeleteProgram(_shaderAzul.Handle);
      GL.DeleteProgram(_shaderCiano.Handle);
      GL.DeleteProgram(_shaderMagenta.Handle);
      GL.DeleteProgram(_shaderAmarela.Handle);

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
