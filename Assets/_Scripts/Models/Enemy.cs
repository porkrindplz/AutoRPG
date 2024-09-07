using System;
using System.Collections.Generic;

namespace _Scripts.Models
{
    public class Enemy : Entity
    {
        public string Name;
        public List<string> Actions;
        public List<double> ActionWeights;
        public string SpritePath;
    }
}