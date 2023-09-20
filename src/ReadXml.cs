using System;
using System.Reflection;
using System.Xml.Linq;
using Microsoft.VisualBasic;

namespace XML_Visualizer;
public class XmlReader
{
    public XmlReader()
    {
    }

    public void Read()
    {
        try
        {
            XDocument topologyXml = XDocument.Load("Fake Data Format/topology/topology.xml");//byt till parameter

            var computers = from computer in topologyXml.Descendants("Computer")
                select new
                {                
                    Name = computer.Attribute("name").Value,   
                    partitions = from partition in computer.Elements("Partition")
                        select new
                        {
                            Name = partition.Attribute("name").Value,
                            partition = computer.Elements("Partition")
                                            .Select(part => part.Attribute("name").Value),
                        
                            applications = from application in partition.Elements("Application")
                            select new
                            {
                                Name = application.Attribute("name").Value,
                                application = partition.Elements("Application")
                                                .Select(app => app.Attribute("name").Value)
                            }
                        }  
                };
 
            foreach (var computer in computers)
            {               
                foreach (var partition in computer.partitions)
                {                   
                    foreach (var application in partition.applications)
                    {
                        try
                        {
                            XDocument applicationXml = XDocument.Load("Fake Data Format/applications/"+application.Name+"/application.xml");

                            var threads = from thread in applicationXml.Descendants("Thread")
                                select new
                                {                
                                    Name = thread.Attribute("name").Value,  
                                    Frequency = thread.Attribute("frequency").Value.Remove(thread.Attribute("frequency").Value.Length -2),
                                    Ports = from Port in thread.Elements("Port")
                                        select new
                                        {                                            
                                            Name = Port.Attribute("name").Value,
                                            Interface = Port.Attribute("interface").Value,    
                                            Role = Port.Attribute("role").Value,    
                                            Port = thread.Elements("Port")
                                                            .Select(_Port => _Port.Attribute("name").Value)                                                                        
                                        }  
                                };
                            //read "Fake Data Format/Application/<AppName>/Resources.xml
                            //read "Fake Data Format/connections/connections.xml

                            //call component constructor from here?
                            
                            Console.WriteLine($"Name: {computer.Name}");                               
                            Console.WriteLine($"  Partition: {partition.Name}");
                            Console.WriteLine($"    Application: {application.Name}");
                            foreach (var thread in threads)
                            {
                                Console.WriteLine($"    Thread: {thread.Name}");
                                Console.WriteLine($"    Frequency: {thread.Frequency}");
                                foreach (var Port in thread.Ports)
                                {
                                    Console.WriteLine($"      Port: {Port.Name}");
                                    Console.WriteLine($"        Interface: {Port.Interface}");
                                    Console.WriteLine($"        Role: {Port.Role}");
                                    
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    }   
                }
            }
        }     
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }
}