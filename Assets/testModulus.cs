using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testModulus : MonoBehaviour {



    public List<int> intList = new List<int>() {2,3,4,5};


	// Use this for initialization
	void Start ()
    {
        List<Vector2> vectArray = new List<Vector2>() {
            new Vector2(2,0),
            new Vector2(2,1),
            new Vector2(3,0),
            new Vector2(3,1),
            new Vector2(3,2),
            new Vector2(4,0),
            new Vector2(4,1),
            new Vector2(4,2),
            new Vector2(4,3),
            new Vector2(5,0),
            new Vector2(5,1),
            new Vector2(5,2),
            new Vector2(5,3),
            new Vector2(5,4)
        };

        //for(int i = 0; i < intList.Count; i++)
        //{
        //    int count = intList[i];
        //    print("shape " + i + " " + count);

        //    for (int j = 0; j < count; j++)
        //    {
        //        int mod = j;
        //        if(i > 0)
        //        {
        //            for(int k = 0; k < intList[i-1]; k++)
        //            {
        //                mod += intList[k]; 
        //                print(mod);
        //            }
        //        }
        //        print( vectArray[mod] );
        //    }
        //}



        int sum = 0;       
        foreach(int s in intNewList)
        {
            sum += s;
        }

        int min = 0;
        int max = intNewList[0];
        int dif = max - min;
        int difplus = 0;
        for(int i = 0; i < intNewList.Count; i++)
        {
            for (int o = min; o < max; o++)
            {
                print("node " + o + " takes position " + difplus + " on shape " + i);
                difplus++;
            }

            min = max;
            dif = max - min;
            difplus = 0;
            if (i < intNewList.Count-1) max += intNewList[i+1];
        }
        
    }

    public List<int> intNewList = new List<int>() { 1, 2, 3, 4 };

    // Update is called once per frame
    void Update () {
		
	}
}
