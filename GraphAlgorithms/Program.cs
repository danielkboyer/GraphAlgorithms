using System;
using System.Collections.Generic;
using System.Linq;

namespace GraphAlgorithms
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            Node[] directedAdjList = 
            {
                new Node(0,1,4,5),
                new Node(1,2,3,4),
                new Node(2,3),
                new Node(3,4,1,2,0),
                new Node(4,5,1,3),
                new Node(5,1,2,3,4)
            };

            Console.WriteLine("S to V : "+SToAllV(0,directedAdjList));
            Console.WriteLine("All V to S: " + AllVToS(0, directedAdjList));


            Node[] unDirectedAdjList =
            {
                new Node(0,1,3),
                new Node(1,0,2,4),
                new Node(2,1,5),
                new Node(3,0,4),
                new Node(4,3,5,1),
                new Node(5,4,2)
            };

            Console.WriteLine("Shortest S To V Count: "+ShortestSToVCount(0, 5, unDirectedAdjList));


            Node[] dagAdjList =
            {
                new Node(0,1,2,3),
                new Node(1,2),
                new Node(2,3),
                new Node(3)
            };

            Console.WriteLine("All Paths from S To V: \n" + PathsFromSToV(0, 3, dagAdjList));
        }

        static bool AllVToS(int sIndex, Node[] adjList)
        {
            //let's transpose the graph and call SToAllV
            Node[] newAdjList = new Node[adjList.Length];
            //O(n+m), it is just looping through each node and its adjacency list and creating a new adjency list.
            foreach(var node in adjList)
            {
                newAdjList[node.Index] = new Node(node.Index);
                foreach(var nodeAdj in node.connectedNodes)
                {
                    if(newAdjList[nodeAdj] == null)
                    {
                        newAdjList[nodeAdj] = new Node(nodeAdj, node.Index);
                    }
                    else
                    {
                        newAdjList[nodeAdj].connectedNodes.Add(node.Index);
                    }
                }
            }

            return SToAllV(sIndex, newAdjList);
        }




        
        static bool SToAllV(int sIndex, Node[] adjList)
        {
            //O(1)
            Queue<Node> queue = new Queue<Node>();
            //O(1)
            adjList[sIndex].Visited = true;
            //O(1)
            adjList[sIndex].Explored = true;
            //O(1)
            queue.Enqueue(adjList[sIndex]);
            //Since a node can only be queued once, and we only visit that nodes edges once,

            //the time complexity is O(m+n)
            do
            {
                //O(1)
                var currentNode = queue.Dequeue();
                //O(1)
                currentNode.Explored = true;
                //O(node.edges)
                foreach(var nodeIndex in currentNode.connectedNodes)
                {
                    if(adjList[nodeIndex].Visited == false)
                    {
                        adjList[nodeIndex].Visited = true;
                        queue.Enqueue(adjList[nodeIndex]);
                    }
                }
            } while (queue.Count > 0);

            //Check if each node in the adjList has been visited or not
            //O(n)
            foreach(var node in adjList)
            {
                if (!node.Visited)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// Since it is a DAG, we don't have to worry about cycles! :)
        /// </summary>
        /// <param name="sIndex"></param>
        /// <param name="endIndex"></param>
        /// <param name="adjList"></param>
        /// <returns></returns>
        static string PathsFromSToV(int sIndex, int endIndex, Node[] adjList)
        {
          
            //O(1)
            Queue<Node> queue = new Queue<Node>();
            //O(1)
            adjList[sIndex].Visited = true;

          
            queue.Enqueue(adjList[sIndex]);
            //Since a node can only be queued once, and we only visit that nodes edges once,
            //the time complexity is O(m+n)
            do
            {
                //O(1)
                var currentNode = queue.Dequeue();
                //O(1)
                currentNode.Explored = true;
                //O(node.edges)
                foreach (var nodeIndex in currentNode.connectedNodes)
                {


                    //add the current node to the nodes previous node index.
                    adjList[nodeIndex].PreviousNodes.Add(currentNode.Index);
                    //if the adjList contains the endindex, we can end our search here and break out of the loop.
                    if (nodeIndex == endIndex)
                    {
                        break;
                    }


                    //We haven't reached the end index yet, add all available nodes to the queue.
                    //we only add nodes that have not been visited.
                    if (adjList[nodeIndex].Visited == false)
                    {
                        adjList[nodeIndex].Visited = true;
                        queue.Enqueue(adjList[nodeIndex]);
                    }
                   
                }
            } while (queue.Count > 0);


            //Here we need to go from the end node and traverse backwards giving us all paths
            //prints the paths backwards
            //O(n) of paths
            return RecursePathTree(adjList,adjList[endIndex], "");
            
       
        }

        //prints the paths backwards
        //O(n) of paths
        static string RecursePathTree(Node[] adjList,Node end, string nodesOnList)
        {
            //base case
            if (end.PreviousNodes.Count == 0)
                return nodesOnList+end.Index+";";

            string listOfLists = "";
            foreach (var previousNode in end.PreviousNodes)
            {

                listOfLists += RecursePathTree(adjList, adjList[previousNode], nodesOnList+end.Index+",");
            }
            return listOfLists;

        }
        static int ShortestSToVCount(int sIndex, int endIndex ,Node[] adjList)
        {
            int shortestPathCount = 0;
            int shortestPath = int.MaxValue;

            //O(1)
            Queue<Node> queue = new Queue<Node>();
            //O(1)
            adjList[sIndex].Visited = true;
            //O(1)
            adjList[sIndex].Explored = true;

            //O(1)
            adjList[sIndex].GetToCount = 1;
            //O(1) the starting reachedByCount must be one that way we can pass it along appropiately.
            adjList[sIndex].ReachedByCount = 1;
            //O(1)
            queue.Enqueue(adjList[sIndex]);
            //Since a node can only be queued once, and we only visit that nodes edges once,
            //the time complexity is O(m+n)
            do
            {
                //O(1)
                var currentNode = queue.Dequeue();
                //O(1)
                currentNode.Explored = true;
                //O(node.edges)
                foreach (var nodeIndex in currentNode.connectedNodes)
                {
                    //if the adjList contains the endindex, we can end our search here and break out of the loop.
                    //We need to check if it's less than the shortest path or equal to and update counts.
                    if(nodeIndex == endIndex)
                    {
                        if(currentNode.GetToCount + 1 == shortestPath)
                        {
                            shortestPathCount+= currentNode.ReachedByCount;
                        }
                        else if(currentNode.GetToCount + 1 < shortestPath)
                        {
                            shortestPathCount = currentNode.ReachedByCount;
                            shortestPath = currentNode.GetToCount + 1;
                        }
                        break;
                    }
                    //We haven't reached the end index yet, add all available nodes to the queue.
                    //we only add nodes that have not been visited.
                    if (adjList[nodeIndex].Visited == false)
                    {
                        adjList[nodeIndex].Visited = true;
                        //This tracks the the count to get to the node
                        adjList[nodeIndex].GetToCount = currentNode.GetToCount+1;
                        //add the reached by count (this is the special change to BFS)
                        adjList[nodeIndex].ReachedByCount+= currentNode.ReachedByCount;
                        queue.Enqueue(adjList[nodeIndex]);
                    }
                    //if the count to get to this node is the same as what is already currently visited,
                    //then we must update the reached to count. This means two nodes with the same count,
                    //both have this node in their adj list.
                    else if(adjList[nodeIndex].GetToCount == currentNode.GetToCount + 1)
                    {
                        adjList[nodeIndex].ReachedByCount+= currentNode.ReachedByCount;
                    }
                }
            } while (queue.Count > 0);

          

            return shortestPathCount;
        }


        class Node
        {
            public int Index { get; set; }

            public bool Visited { get; set; }
            public bool Explored { get; set; }

            //the count it took to get to this node from the start.
            public int GetToCount { get; set; }

            //the amount of nodes that can reach this node in the GetToCount
            
            public int ReachedByCount { get; set; }
            public List<int> connectedNodes { get; set; }


            public List<int> PreviousNodes { get; set; } = new List<int>();

            public Node(int Index, params int[] adjList)
            {
                this.Index = Index;
                this.connectedNodes = adjList.ToList();
            }
        }
    }
}
