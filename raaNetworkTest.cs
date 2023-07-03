using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class raaNetworkTest : MonoBehaviour {

    raaAsyncServer s = null;


    // Use this for initialization
    void Start () {
        s = new raaAsyncServer(19723, incommingEvent);
	}

    public void incommingEvent(raaEvent e)
    {
        if (e.msg() != null)
        {
            raaMessage m = e.msg();
            m.print();
        }
        else
        {
            Debug.Log("Event");
        }
    }



    // Update is called once per frame
    void Update () {
        if (Input.GetKeyDown("space"))
        {

            raaMessage m = new raaMessage();
            m.add("Hi Robbie");
            s.send(m);

            print("space key was pressed");
        }

        s.processEvents();
    }
}
