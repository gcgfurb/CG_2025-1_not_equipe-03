#define CG_Debug
using CG_Biblioteca;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using System.Drawing;

namespace gcgcg
{
    internal class Face : Objeto
    {
        private static Ponto4D[] _textureVertices = new[]
        {
            new Ponto4D(1, 1),
            new Ponto4D(1, 0),
            new Ponto4D(0, 1),
            new Ponto4D(0, 0),
        };

        private static Ponto4D[] _normalVertices = new[]
        {
            new Ponto4D( 0,  0,  1),
            new Ponto4D( 1,  0,  0),
            new Ponto4D( 0,  0, -1),
            new Ponto4D(-1,  0,  0),
            new Ponto4D( 0, -1,  0),
            new Ponto4D( 0,  1,  0),
        };

        public Face(Objeto _paiRef, ref char _rotulo, Ponto4D[] vertices)
            : this(_paiRef, ref _rotulo, vertices, null, null)
        {
        }

        public Face(Objeto _paiRef, ref char _rotulo, Ponto4D[] vertices, int[] indicesTexturas, int[] indicesNormais)
            : base(_paiRef, ref _rotulo)
        {
            PrimitivaTipo = PrimitiveType.TriangleFan;
            PrimitivaTamanho = 10;

            AdicionarPontosVertices(vertices);

            AdicionarPontosTexturas(indicesTexturas);

            AdicionarPontosNormais(indicesNormais);

            Atualizar();
        }

        private void AdicionarPontosVertices(Ponto4D[] vertices)
        {
            foreach (var vertice in vertices)
            {
                base.PontosAdicionar(vertice);
            }
        }

        private void AdicionarPontosTexturas(int[] indicesTexturas)
        {
            if (indicesTexturas == null)
                return;

            foreach (var indiceTextura in indicesTexturas)
            {
                pontosTextura.Add(_textureVertices[indiceTextura]);
            }
        }

        private void AdicionarPontosNormais(int[] indicesNormais)
        {
            if (indicesNormais == null)
                return;

            foreach (var indiceNormal in indicesNormais)
            {
                pontosNormal.Add(_normalVertices[indiceNormal]);
            }
        }

        private void Atualizar()
        {
            base.ObjetoAtualizar();
        }

#if CG_Debug
        public override string ToString()
        {
            string retorno;
            retorno = "__ Objeto Face _ Tipo: " + PrimitivaTipo + " _ Tamanho: " + PrimitivaTamanho + "\n";
            retorno += base.ImprimeToString();
            return (retorno);
        }
#endif

    }
}