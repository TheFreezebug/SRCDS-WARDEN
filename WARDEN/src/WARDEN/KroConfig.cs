using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WARDEN
{
    public class KroConfig
    {
        private Dictionary<string, string> config_data;
        private string config_rom_file;


        private bool readConfig(string config_rom)
        {
            string contents;
            try
            {
                
                string[] ConfigurationRows = File.ReadAllLines(config_rom_file);
                foreach (string row in ConfigurationRows)
                {
                    string[] ConfigItem = row.Split(new char[1]{(char)0x3D}, 2);
                    if (ConfigItem.Length > 1)
                    {
                       config_data[ConfigItem[0]] = ConfigItem[1];
                      Console.WriteLine("{0} = {1}",ConfigItem[0],ConfigItem[1]);
                    }
                    else
                    {
                       // Console.WriteLine("Corrupt configuration item: " + ConfigItem[0]);
                    }
                }

                return true;

            } catch ( Exception E)
            {
                Console.WriteLine(E.ToString());
                return false;
            }
        }

        public string getValue(string val,string def)
        {
            string Ret = null;
            config_data.TryGetValue(val, out Ret);
            if (Ret==null)
            {
                return def;
            }
            return Ret;
        }

        public bool setValue(string key, string def)
        {
            try
            {
                config_data[key] = def;
                return true;
            } catch
            {
                return false;
            }
        }

        public bool saveConfig()
        {
            File.WriteAllText(config_rom_file, "");
            foreach (KeyValuePair<string, string> entry in config_data)
            {
                File.AppendAllText(config_rom_file, entry.Key + "=" + entry.Value + "\r\n");

            }

            return true;
        }

        public KroConfig(string config_rom)
        {
            config_rom_file = config_rom;
            config_data = new Dictionary<string, string>();
            bool success = readConfig(config_rom_file);

            if (!success)
            {
                Console.WriteLine("Couldn't load file {0}. Configuration will be blank.", config_rom_file);
            }
        }
    }
}
;