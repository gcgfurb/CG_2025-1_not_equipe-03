#define CG_Debug
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace gcgcg
{
  internal class Cubo : Objeto
  {
    Ponto4D[] vertices;

    public Cubo(Objeto _paiRef, ref char _rotulo) : base(_paiRef, ref _rotulo)
    {
      PrimitivaTipo = PrimitiveType.Triangles;
      PrimitivaTamanho = 36; 

      vertices = new Ponto4D[]
      {
        new Ponto4D(-1.0f, -1.0f,  1.0f), // 0
        new Ponto4D( 1.0f, -1.0f,  1.0f), // 1
        new Ponto4D( 1.0f,  1.0f,  1.0f), // 2
        new Ponto4D(-1.0f,  1.0f,  1.0f), // 3
        new Ponto4D(-1.0f, -1.0f, -1.0f), // 4
        new Ponto4D( 1.0f, -1.0f, -1.0f), // 5
        new Ponto4D( 1.0f,  1.0f, -1.0f), // 6
        new Ponto4D(-1.0f,  1.0f, -1.0f)  // 7
      };

      // Face Frente
      base.PontosAdicionar(vertices[0]); 
      base.PontosAdicionar(vertices[1]);
      base.PontosAdicionar(vertices[2]);
      
      base.PontosAdicionar(vertices[0]); 
      base.PontosAdicionar(vertices[2]);
      base.PontosAdicionar(vertices[3]);

      // Face Fundo
      base.PontosAdicionar(vertices[5]); 
      base.PontosAdicionar(vertices[4]);
      base.PontosAdicionar(vertices[7]);
      
      base.PontosAdicionar(vertices[5]); 
      base.PontosAdicionar(vertices[7]);
      base.PontosAdicionar(vertices[6]);

      // Face Cima
      base.PontosAdicionar(vertices[3]); 
      base.PontosAdicionar(vertices[2]);
      base.PontosAdicionar(vertices[6]);
      
      base.PontosAdicionar(vertices[3]); 
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[7]);

      // Face baixo
      base.PontosAdicionar(vertices[4]); 
      base.PontosAdicionar(vertices[5]);
      base.PontosAdicionar(vertices[1]);
      
      base.PontosAdicionar(vertices[4]); 
      base.PontosAdicionar(vertices[1]);
      base.PontosAdicionar(vertices[0]);

      // Face direita
      base.PontosAdicionar(vertices[1]); 
      base.PontosAdicionar(vertices[5]);
      base.PontosAdicionar(vertices[6]);
      
      base.PontosAdicionar(vertices[1]); 
      base.PontosAdicionar(vertices[6]);
      base.PontosAdicionar(vertices[2]);

      // Face esquerda
      base.PontosAdicionar(vertices[4]); 
      base.PontosAdicionar(vertices[0]);
      base.PontosAdicionar(vertices[3]);
      
      base.PontosAdicionar(vertices[4]); 
      base.PontosAdicionar(vertices[3]);
      base.PontosAdicionar(vertices[7]);

      Atualizar();
    }

    private void Atualizar()
    {
      base.ObjetoAtualizar();
    }

#if CG_Debug
    public override string ToString()
    {
      string retorno;
      retorno = "__ Objeto Cubo _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
      retorno += base.ImprimeToString();
      return (retorno);
    }
#endif

  }
}