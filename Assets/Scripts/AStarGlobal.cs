using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AStarGlobal : MonoBehaviour
{
    private GameObject[] _nodes;

    private void Start()
    {
        _nodes = GameObject.FindGameObjectsWithTag("Node");
    }

    public List<Node> FindShortestPath(Node start, Node end)
    {

        if (AStarAlgorithm(start, end))
        {
            List<Node> result = new List<Node>();

            Node current = end;

            do
            {
                result.Insert(0, current);
                current = current.PreviousNode;
            }
            while (current != null);

            return result;
        }
        return null;
    }

    private bool AStarAlgorithm(Node start, Node end)
    {
        List<Node> unexplored = new List<Node>();

        foreach (GameObject obj in _nodes)
        {
            if (obj.TryGetComponent<Node>(out Node n))
            {
                if (!n.Disabled)
                {
                    n.ResetNode();
                    unexplored.Add(n);
                    n.SetDirectDistanceToEnd(end.transform.position);
                }
            }
        }

        if (!unexplored.Contains(start) && !unexplored.Contains(end))
        {
            return false;
        }

        start.PathWeight = 0;
        while (unexplored.Count > 0)
        {
            //order based on path
            unexplored.Sort(
                (x, y) => x.PathWeightHeuristic.CompareTo(y.PathWeightHeuristic)
                );

            //current is the currently shortest path possible
            Node current = unexplored[0];

            if (current == end)
                break;

            unexplored.RemoveAt(0);

            foreach (Node neighbour in current.NeighbourNodes)
            {
                if (!unexplored.Contains(neighbour))
                    continue;

                float distance = Vector3.Distance(current.transform.position, neighbour.transform.position);
                distance += current.PathWeight;

                if (distance < neighbour.PathWeight)
                {
                    neighbour.PathWeight = distance;
                    neighbour.PreviousNode = current;
                }
            }
        }

        return true;
    }
}