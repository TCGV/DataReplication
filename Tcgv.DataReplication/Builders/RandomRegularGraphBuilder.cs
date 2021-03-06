﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tcgv.DataReplication.DataModel;

namespace Tcgv.DataReplication.Builders
{
    public class RandomRegularGraphBuilder : GraphBuilder
    {
        public override Graph Build(List<Vertex> vertices, int r)
        {
            ValidateParameters(vertices.Count, r);

            for (int i = 0; i < maxGraphIteration; i++)
            {
                Initialize(vertices.Count, r);
                var g = BuildCandidate();
                if (IsRRegular(g, r))
                    return new Graph(vertices.ToArray(), g);
            }

            return null;
        }

        private void Initialize(int n, int r)
        {
            groups = new List<List<int>>();
            pairs = new List<Point>();

            u = new List<int>();
            for (int i = 1; i < n * r + 1; i++)
                u.Add(i);

            for (int i = 0; i < n; i++)
            {
                var group = new List<int>();
                for (int j = i * r; j < i * r + r; j++)
                    group.Add(u[j]);
                groups.Add(group);
            }
        }

        private HashSet<int>[] BuildCandidate()
        {
            for (int i = 0; i < maxPairIteration && u.Count > 1; i++)
                PairNewPoint();
            return BuildGraph();
        }

        private bool IsRRegular(HashSet<int>[] g, int r)
        {
            var c = 0;
            for (int i = 0; i < g.Length; i++)
            {
                if (g[i].Count != r && c++ > 0)
                    return false;
            }
            return true;
        }

        private HashSet<int>[] BuildGraph()
        {
            var g = new HashSet<int>[groups.Count];
            for (int i = 0; i < g.Length; i++)
                g[i] = new HashSet<int>();

            for (int i = 0; i < pairs.Count; i++)
            {
                var v = pairs[i];
                g[v.X].Add(v.Y);
                g[v.Y].Add(v.X);
            }
            return g;
        }

        private void PairNewPoint()
        {
            var point = GetRandomPoint();

            if (IsSuitable(point))
            {
                var p = new Point(
                    GetGroupIndexOfValue(point.X),
                    GetGroupIndexOfValue(point.Y)
                );
                pairs.Add(p);
                u.Remove(point.Y);
                u.Remove(point.X);
            }
        }

        private bool IsSuitable(Point point)
        {
            var groupX = GetGroupIndexOfValue(point.X);
            var groupY = GetGroupIndexOfValue(point.Y);

            if (groupX == groupY)
                return false;

            if (PairExists(groupX, groupY))
                return false;

            return true;
        }

        private Point GetRandomPoint()
        {
            var x = rd.Next(u.Count);

            while (true)
            {
                var y = rd.Next(u.Count);

                if (x != y)
                {
                    if (x < y)
                        return new Point(u[x], u[y]);
                    else
                        return new Point(u[y], u[x]);
                }
            }
        }

        private bool PairExists(int x, int y)
        {
            return pairs.Any(p => p.X == x && p.Y == y);
        }

        private int GetGroupIndexOfValue(int v)
        {
            for (int i = 0; i < groups.Count; i++)
                for (int j = 0; j < groups[i].Count; j++)
                    if (groups[i][j] == v)
                        return i;
            return -1;
        }

        List<int> u;
        List<List<int>> groups;
        List<Point> pairs;

        private static int maxPairIteration = 100000;
        private static int maxGraphIteration = 100;
        private static Random rd = new Random(Guid.NewGuid().GetHashCode());
    }
}
