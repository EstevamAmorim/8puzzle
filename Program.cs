using System;
using System.Collections.Generic;

namespace _8puzzle
{
    public class Program
    {
        enum MaxDepth : int
        {
            DepthSearch = 24,
            WidthSearch = 24,
            GreedySearch = 24,
            ASearch = 24
        }

        public static int ASearchMax = (int)Math.Pow(1.4, (double)MaxDepth.ASearch);
        
        public static List<(int movement, int reverse)> moves = new List<(int, int)> { (1, 2), (2, 1), (3, 4), (4, 3) };

        public class NextChoice
        {
            public TabState choice;
            public int weight;

            public NextChoice(TabState choice, int weight)
            {
                this.choice = choice;
                this.weight = weight;
            }
        }
        
        static void DepthSearch(TabState current_state)
        {   
            if (current_state.GetDepth() > (int)MaxDepth.DepthSearch || current_state.CheckSolution())
                return;
            
            foreach((int movement, int reverse) move in moves)
            {
                if (current_state.GetLast() != move.reverse && current_state.Move(move.movement))
                {
                    DepthSearch(current_state);

                    if (current_state.CheckSolution())
                        return;

                    current_state.Revert();
                }
            }
        }

        static void WidthSearch(TabState tab)
        {
            int last;
            TabState queue_first;

            Queue<TabState> tab_queue = new Queue<TabState>();
            tab_queue.Enqueue(tab);

            while(tab_queue.Peek().GetDepth() < (int)MaxDepth.WidthSearch)
            {
                queue_first = tab_queue.Peek();
                last = queue_first.GetLast();

                foreach ((int movement, int reverse) move in moves)
                {
                    if (last != move.reverse && queue_first.Move(move.movement))
                    {
                        if (queue_first.CheckSolution())
                        {
                            tab.Update(queue_first);
                            tab_queue.Clear();
                            return;
                        }

                        tab_queue.Enqueue((TabState)queue_first.Clone());
                        queue_first.Revert();
                    }
                }

                tab_queue.Dequeue();
            }

            tab_queue.Clear();
        }

        static void GreedySearch(TabState current_state)
        {
            int min = 1000;
            int md, next_move = 0;

            if (current_state.GetDepth() > ASearchMax || current_state.CheckSolution())
                return;
            
            foreach ((int movement, int reverse) move in moves)
            {
                if (current_state.GetLast() != move.reverse && current_state.Move(move.movement))
                {
                    if (current_state.CheckSolution())
                        return;

                    md = current_state.ManhattanDistance();

                    if (md < min)
                    {
                        min = md;
                        next_move = move.movement;
                    }
                        
                    current_state.Revert();
                }
            }

            current_state.Move(next_move);
            GreedySearch(current_state);
        }

        static void ASearch(TabState current_state)
        {
            TabState aux = (TabState)current_state.Clone();
            int weight, index = 0;
            int value = 9999;
            List<NextChoice> choices = new List<NextChoice>();
            
            while (choices.Count < ASearchMax || current_state.CheckSolution())
            {
                if (current_state.GetDepth() < (int)MaxDepth.ASearch)
                {
                    foreach ((int movement, int reverse) move in moves)
                    {
                        if (current_state.GetLast() != move.reverse && current_state.Move(move.movement))
                        {
                            if (current_state.CheckSolution())
                            {
                                choices.Clear();
                                return;
                            }

                            weight = current_state.ManhattanDistance() + current_state.GetDepth();

                            choices.Add(new NextChoice((TabState)current_state.Clone(), weight));
                            current_state.Revert();
                        }
                    }
                }

                for (int i = 0; i < choices.Count; i++)
                {
                    if (choices[i].weight < value)
                    {
                        value = choices[i].weight;
                        index = i;
                    }
                }
  
                current_state.Update(choices[index].choice);
                choices.RemoveAt(index);
            }

            current_state.Update(aux);
            choices.Clear();
        }

        static void Main(string[] args)
        {
            TabState.InitSolution(new int[,] { { 1, 2, 3 }, { 4, 0, 5 }, { 6, 7, 8 } });

            //TabState t = new TabState(new int[,] { {1, 2, 3}, {4, 0, 5}, {6, 8, 7} });
            //TabState t = new TabState(new int[,] { {0, 7, 2 }, { 1, 4, 3 }, { 6, 8, 5 } });
            TabState t = new TabState(new int[,] { { 4, 1, 2 }, { 0, 5, 3 }, { 6, 7, 8 } });
            //TabState t = new TabState(new int[,] { { 1, 2, 3 }, { 4, 7, 5 }, { 6, 8, 0 } });

            DepthSearch(t);
            
            //WidthSearch(t);

            //GreedySearch(t);

            //ASearch(t);

            //GreedySearch(t);

            Console.WriteLine(t.ToString());
        }
    }
}
