using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Candidate : MonoBehaviour
{
  
        public string candidate_id;
        public string userName;
        public string gold;
        public string silver;
        public string black;
        public string currentAvatarId;
        public List<(string roleName, string count)> previousStatusInformation = new List<(string roleName, string count)>();
}
