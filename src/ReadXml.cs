using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;

namespace XML_Visualizer;
class XmlReader {

   public XmlReader()
   {

   }
    public List <Computer> ReadXml() {
        String line;
        List <Computer> computers = new List<Computer>();
        try
        {
            List <Partition> partitions = new List<Partition>();
            List <Application> applications = new List<Application>();
            List <Thread> threads = new List<Thread>();
            string applicationName;
            int ramSize = 0;
            int initStack = 0;

            StreamReader topologyReader = new StreamReader("Fake Data Format/topology/topology.xml");
            line = topologyReader.ReadLine(); //
            while (line != null)
            {
                line = line.Trim();
                if(line.Contains('\"')){
                    if (line.Split('\"')[0] == "<Computer name=" ) {
                        partitions.Clear();
                        computers.Add(new Computer (line.Split('\"')[1]));
                    } else if (line.Split('\"')[0] == "<Partition name=" ) {
                        applications.Clear();
                        partitions.Add(new Partition (line.Split('\"')[1]));
                    }else if (line.Split('\"')[0] == "<Application name=" ) {
                        threads.Clear();
                        applicationName = (line.Split('\"')[1]);
                        ReadResourses( applicationName, ref threads, ref ramSize, ref initStack);
                        ReadApplication(applicationName, ref threads);
                   //     applications.Add(new Application(applicationName, threads, ramSize, initStack));
                    }
                }else if (line == "</Computer>"){
                 //   computers[computers.Count-1].SetChildren(partitions);
                }else if (line == "</Partition>"){
                 //    partitions[partitions.Count-1].SetChildren(applications);
                }

                line = topologyReader.ReadLine();
            }
            topologyReader.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        return computers;
    }

    void ReadApplication(string applicationName, ref List<Thread> threads) {
        String line;
        try
        {
            int index = 0;
            string name;
            string interf;//interface
            string role;
            List<Port> ports = new List<Port> ();
            StreamReader applicationReader = new StreamReader("Fake Data Format/applications/"+applicationName+"/application.xml");
            line = applicationReader.ReadLine();
            
            while (line != null)
            {
                line = line.Trim();
                if(line.Contains('\"')){ 
                    if (line.Split('\"')[0] == "<Thread name=" ) {
                        threads[index].SetFrequency(Int32.Parse(line.Split('\"')[3]));
                        index ++;
                    } else if (line.Split('\"')[0] =="<Port name=" ) {
                        name = (line.Split('\"')[1]);
                        interf = (line.Split('\"')[3]);
                        role = (line.Split('\"')[5]);
                        ports.Add(new Port (name, interf, role));
                    }
                }else if (line == "</Thread>"){
                  //  threads[threads.Count-1].SetChildren(ports);
                }
                line = applicationReader.ReadLine();
            }
            applicationReader.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
    }

   private void ReadResourses(string applicationName, ref List<Thread> threads, ref int ramSize, ref int initStack) {
        string line;
        StreamReader ResourcesReader = new StreamReader("Fake Data Format/applications/"+applicationName+"/resources.xml");
        line = ResourcesReader.ReadLine();
        while (line != null)
        {
            line = line.Trim();
            if (line.Length > 10) {
                if (line.Split(' ')[0] == "<ApplicationResourceRequirements" ) {
                    initStack = Int32.Parse(line.Split('\"')[3]);
                    ramSize = Int32.Parse(line.Split('\"')[4]);
                } else if (line.Split(' ')[0] == "<ThreadResourceRequirements" ) {
                    threads.Add(new Thread (line.Split('\"')[1], Int32.Parse(line.Split('\"')[4]), Int32.Parse(line.Split('\"')[3])));
                }
            }
            line = ResourcesReader.ReadLine();
        }
    }

}

