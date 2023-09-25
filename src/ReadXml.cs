using System;
using System.Collections.Specialized;
using System.Data;
using System.IO;

namespace XML_Visualizer;
class XmlReader {

    public XmlReader()
    {
    }
    public List<Component> ReadXml() {
        String line;
        List <Component> computers = new List<Component>();
        int activeThread = 0;
        try
        {
            List <Component> partitions = new List<Component>();
            List <Component> applications = new List<Component>();
            List <Component> threads = new List<Component>();
            string applicationName;
            int ramSize = 0;
            int initStack = 0;

            StreamReader topologyReader = new StreamReader("Fake Data Format/topology/topology.xml");
            line = topologyReader.ReadLine();
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
                        ReadResourses(applicationName, threads, ref ramSize, ref initStack);
                        ReadApplication(applicationName, threads, activeThread);
                        applications.Add(new Application(applicationName, ramSize, initStack));
                        applications[applications.Count-1].SetChildren(threads);
                    }
                }else if (line == "</Computer>"){
                    computers[computers.Count-1].SetChildren(partitions);
                }else if (line == "</Partition>"){
                    partitions[partitions.Count-1].SetChildren(applications);
                }

                line = topologyReader.ReadLine();
            }
            topologyReader.Close();
        }
        catch(Exception e)
        {
            Console.WriteLine("Exception: " + e.Message);
        }
        //debugPrint(computers);
        return computers;
    }

    void ReadApplication(string applicationName, List<Component> threads, int activeThread) {
        String line;
        Console.WriteLine("Fake Data Format/applications/"+applicationName+"/application.xml");
        try
        {
            int index = 0;
            string name;
            string interf;
            string role;
            List<Component> ports = new List<Component> ();
            StreamReader applicationReader = new StreamReader("Fake Data Format/applications/"+applicationName+"/application.xml");
            line = applicationReader.ReadLine();
            
            while (line != null)
            {
                line = line.Trim();
                if(line.Contains('\"')){ 
                    if (line.Split('\"')[0] == "<Thread name=" ) {
                        ((Thread)threads[index]).SetFrequency(Int32.Parse(line.Split('\"')[3].Remove(line.Split('\"')[3].Length-2)));
                        index ++;
                    } else if (line.Split('\"')[0] =="<Port name=" ) {
                        name = (line.Split('\"')[1]);
                        interf = (line.Split('\"')[3]);
                        role = (line.Split('\"')[5]);
                        ports.Add(new Port (name, interf, role));
                    }
                }else if (line == "</Thread>"){
                    threads[activeThread].SetChildren(ports);
                    ports.Clear();
                    index = 0;
                    activeThread++;
                }
                line = applicationReader.ReadLine();
            }
            applicationReader.Close();
        }
        catch(Exception e)
        {
            //Console.WriteLine("Exception: " + e.Message);
        }
    }

   private void ReadResourses(string applicationName, List<Component> threads, ref int ramSize, ref int initStack) {
        string line;
        Console.WriteLine("Fake Data Format/applications/"+applicationName+"/resources.xml");
        try {
            StreamReader ResourcesReader = new StreamReader("Fake Data Format/applications/"+applicationName+"/resources.xml");
            line = ResourcesReader.ReadLine();
            while (line != null)
            {
                line = line.Trim();
                if (line.Length > 10) {
                    if (line.Split(' ')[0] == "<ApplicationResourceRequirements" ) {
                        initStack = Int32.Parse(line.Split('\"')[3]);
                        ramSize = Int32.Parse(line.Split('\"')[5]);
                    } else if (line.Split(' ')[0] == "<ThreadResourceRequirements" ) {
                        threads.Add(new Thread (line.Split('\"')[1], Int32.Parse(line.Split('\"')[5]), Int32.Parse(line.Split('\"')[3])));
                    }
                }
                line = ResourcesReader.ReadLine();
            }
        }
        catch(Exception e)
        {
            //Console.WriteLine("Exception: " + e.Message);
        }
    }

    private void debugPrint(List<Computer> computers) {
        foreach(Computer computer in computers) {
            Console.WriteLine("" + computer.getName());
            foreach(Partition partition in computer.GetChildren()) {
                Console.WriteLine("  " + partition.getName());
                foreach(Application application in partition.GetChildren()) {
                    Console.WriteLine("    " + application.getName());
                    foreach(Thread thread in application.GetChildren()) {
                        Console.WriteLine("      " + thread.getName());
                        foreach(Port port in thread.GetChildren()) {
                           Console.WriteLine("        " + port.getName() + " " + ((Port)port).interf + " " + ((Port)port).role);
                        }
                    }
                }    
            }   
        }
    }
}

