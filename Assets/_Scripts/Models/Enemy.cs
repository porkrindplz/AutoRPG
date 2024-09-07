using System.Collections.Generic;

namespace _Scripts.Models
{
    public class Enemy : Entity
    {
        public string Name { get; set; }
        public List<string> Actions { get; set; }
        public List<double> ActionWeights { get; set; }
    }
}