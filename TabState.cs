using System;
using System.Collections.Generic;
using System.Linq;

namespace _8puzzle
{
    //Classe que representa um estado
    public class TabState : ICloneable
    {
        //estado do jogo como uma matriz, 0 é o espaco em branco
        private int[,] state;

        //coordenadas do espaco em branco
        private int empty_l;
        private int empty_c;

        //estado final a ser buscado
        private static int[,]? solution_template;

        //movimentos realizados para chegar no estado atual, pilha onde o elemento mais ao topo representa
        //o ultimo movimento. 1- up, 2 - down, 3 - right, 4 - left
        private Stack<int> path;

        public TabState(int[,] state)
        {
            this.state = state;
            this.path = new Stack<int>();
            Update0Position();
        }

        //sobrecarga do construtor para permitir clonagem
        private TabState(TabState t)
        {
            state = (int[,])(t.state).Clone();
            this.empty_l = t.empty_l;
            this.empty_c = t.empty_c;
            this.path = new Stack<int>(new Stack<int>(t.path));
        }

        public override string ToString()
        {
            string result = "{";

            for (int i = 0; i < 3; i++)
            {
                result += "{";
                for (int j = 0; j < 3; j++)
                {
                    result += state[i, j].ToString() + ", ";
                }
                result += "}, ";
            }

            return result + "}";
        }

        public object Clone()
        {
            return new TabState(this);
        }

        public static void InitSolution(int[,] s)
        {
            solution_template = new int[9, 2];

            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    solution_template[s[i, j], 0] = i;
                    solution_template[s[i, j], 1] = j;
                }
            }
        }

        //quantidade de elementos em path representa a profundidade do estado na arvore
        public int GetDepth()
        {
            return path.Count;
        }

        //ultimo movimento usado para chegar no estado
        public int GetLast()
        {
            if (path.Count == 0)
                return 0;

            return path.Peek();
        }

        public int ManhattanDistance()
        {
            float manh = 0;
            
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    manh += Math.Abs(solution_template[state[i, j], 0] - i) + Math.Abs(solution_template[state[i, j], 1] - j);
                }
            }

            return (int)manh;
        }

        public void Update(TabState t)
        {
            this.state = (int[,])(t.state).Clone();
            this.empty_l = t.empty_l;
            this.empty_c = t.empty_c;
            this.path = new Stack<int>(new Stack<int>(t.path));
        }

        private void Update0Position()
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (state[i, j] == 0)
                    {
                        empty_l = i;
                        empty_c = j;
                    }
                }
            }
        }

        //reverte o ultimo movimento
        public void Revert()
        {
            if (path.Count != 0)
            {
                switch (path.Pop())
                {
                    case 1:
                        this.DownMove();
                        break;
                    case 2:
                        this.UpMove();
                        break;
                    case 3:
                        this.LeftMove();
                        break;
                    case 4:
                        this.RightMove();
                        break;
                }

                path.Pop();
            }

        }

        //movimentos, a chamada desses metodos reflete a alteracao no atributo state relativa ao movimento
        private bool UpMove()
        {
            if (empty_l != 0)
            {
                state[empty_l, empty_c] = state[empty_l - 1, empty_c];
                state[empty_l - 1, empty_c] = 0;
                empty_l--;
                path.Push(1);

                return true;
            }

            return false;
        }

        private bool DownMove()
        {
            if (empty_l != 2)
            {
                state[empty_l, empty_c] = state[empty_l + 1, empty_c];
                state[empty_l + 1, empty_c] = 0;
                empty_l++;
                path.Push(2);

                return true;
            }

            return false;
        }

        private bool RightMove()
        {
            if (empty_c != 2)
            {
                state[empty_l, empty_c] = state[empty_l, empty_c + 1];
                state[empty_l, empty_c + 1] = 0;
                empty_c++;
                path.Push(3);

                return true;
            }

            return false;
        }

        private bool LeftMove()
        {
            if (empty_c != 0)
            {
                state[empty_l, empty_c] = state[empty_l, empty_c - 1];
                state[empty_l, empty_c - 1] = 0;
                empty_c--;
                path.Push(4);

                return true;
            }

            return false;
        }

        public bool Move(int move)
        {
            if (move == 1)
                return this.UpMove();

            if (move == 2)
                return this.DownMove();

            if (move == 3)
                return this.RightMove();

            if (move == 4)
                return this.LeftMove();

            return false;
        }

        //verifica se o estado atual eh igual ao estado final
        public bool CheckSolution()
        {
            int value_line, value_col;

            for (int value = 0; value < 9; value++)
            {
                value_line = solution_template[value, 0];
                value_col = solution_template[value, 1];

                if (state[value_line, value_col] != value)
                    return false;
            }

            return true;
        }
    }
}
