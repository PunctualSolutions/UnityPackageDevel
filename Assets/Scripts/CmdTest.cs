using PunctualSolutionsTool.Tool;
using UnityEngine;

namespace PunctualSolutions.UnityPackageDevel
{
    public class CmdTest : MonoBehaviour
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            // CmdTool.RunCmd("C:/Users/s_xgx1/Project/CmdTest/CmdTest/bin/Debug/net8.0/CmdTest.exe", "test",false,true);
            CmdTool.RunCmd("C:/Program Files/Mozilla Firefox/firefox.exe");
            print("111");
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}