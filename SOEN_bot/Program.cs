using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;          // The Discord includes are the DISCORD.NET V0.9.6 bot framework. Required for operation and the backbone of the bot.
using Discord.Legacy;
using Discord.Commands;
using Discord.Logging;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args) => new Program().Start();   // Creates a new section of code which will act as the main. Using main directly has some existing problems with the bot framework. 

        private DiscordClient _client;              // Creates the client for the bot;
        Random random = new Random();               // Leftover code from old bot, inits a variable which can be used to generate a random value.

        public void Start()
        {
            _client = new DiscordClient(x =>        // Lambda to contain the init. of the bot's info.
            {
                x.AppName = "SOENBot";              // Defines what the bot's name will be displayed as.
                x.LogLevel = LogSeverity.Info;      // Sets a minimum level of which events to log into the console window running the bot. Info shows only changes in things like: roles, groups, etc...
                x.LogHandler = Log;                 // Defines an event handler to preform the logging.
            });

            _client.UsingCommands(x =>              // Lambda to contain the init. of the bot's core commands.
            {
                x.PrefixChar = '!';                 // Defines what the trigger character will be for commands so bot knows when a word is used in normal text vs. command.
                x.AllowMentionPrefix = false;       // Sets the ability to use @Mentions to the bot with commands. Used mainly if more bots exist on server. 
                //x.HelpMode = HelpMode.Public;     // Sets the option to use the built in help command display with the bot. By default not used. (Used for experimentation purposes only here)
            });

            var token = "Use your own token here";  // A string containing the user connection token which is the unique identifier for the bot. (Removed for security reasons)

            CreateCommands();                       // Function to go to and use user defined commands.

            _client.MessageReceived += async (s, e) =>  // Lambda event setup for when a message is received.
            {
                var channel = e.Server.FindChannels("admin", ChannelType.Text).FirstOrDefault();    // Assigning channel of where to send message to after it matches a specific string search.
                string user = e.User.Name;          // Defines what a user's name is from the last message object received.
                string message = e.Message.ToString();  // Converting the message object received to a string.

                if ((message.IndexOf("&276185070157430784") != -1) && !(e.User.Roles.Any(input => input.Name.ToUpper() == "MODS"))) // Check that @Mods was in msg and was not used by any in @Mods.
                {
                    await channel.SendMessage($"{user} is in need of assistance in {e.Channel}.");  // Send a message from the bot to the channel object with the string contents. 
                }                                                                                   // Note: All async events MUST have at least one await command within them to know where to wait until all prior statements are done.
            };

            _client.ExecuteAndWait(async () =>      // Connection and reconnection method of bot to any invited server using its user ID token.
            {
                await _client.Connect(token, TokenType.Bot);
            });
        }

        public void CreateCommands()
        {
            var cService = _client.GetService<CommandService>();    // Init's service which deals with the execution of custom commands.

            cService.CreateGroup("sb", ins =>       // Creates the top group of commands which all other commands are a part of.
            {

                ins.CreateCommand("help")           // Start of command definition.
                       .Description("Gives syntax on how to use a command, when no command specified it lists all commands. Syntax: !sb help <command>") // Command description. (Used wih HelpMode)
                       .Do(async (e) =>             // Start of command actions to preform.
                       {
                           await e.Channel.SendMessage($"This is the list of following usable commands: \n!sb help \n!sb time \n!sb register \n!sb join <Course> \n!sb leave <Course> \n!sb list");  // Lists all available commands as a message.
                       });

                ins.CreateCommand("time")           // Command to return current EST time as a message.
                   .Description("Gives the current time in -5 EST.")
                   .Do(async (e) =>
                   {
                       DateTime localTime = DateTime.Now;
                       string currTime = $"The current time is: {localTime.ToString()}";
                       await e.Channel.SendMessage(currTime);
                   });

                ins.CreateCommand("join")            // Command to allow users to join any role.
                   .Description("Allows a user to join a role.")
                   .Parameter("role", ParameterType.Required)       // Defines that the next value after the "join" word is to be taken as a REQUIRED parameter. 
                   .Do(async (e) =>
                   {
                       int pos = 0;
                       string[] roles = {"Mods","Omnics","Members","None","COMP-208","COMP-218","COMP-228","COMP-232","COMP-233","COMP-248","COMP-249","COMP-326","COMP-335","COMP-339","COMP-345","COMP-346","COMP-348","COMP-352","COMP-353","COMP-354","COMP-361","COMP-367","COMP-371","COMP-376","COMP-426","COMP-428","COMP-442","COMP-444","COMP-445","COMP-451","COMP-465","COMP-472","COMP-473","COMP-474","COMP-476","COMP-477","COMP-478","COMP-479","COMP-490","COMP-492","COMP-495","COMP-498","SOEN-228","SOEN-287","SOEN-298","SOEN-321","SOEN-331","SOEN-341","SOEN-342","SOEN-343","SOEN-344","SOEN-345","SOEN-357","SOEN-384","SOEN-385","SOEN-387","SOEN-390","SOEN-422","SOEN-423","SOEN-448","SOEN-449","SOEN-487","SOEN-490","SOEN-491","SOEN-498","ENGR-201","ENGR-202","ENGR-213","ENGR-233","ENGR-242", "ENGR-251", "ENGR-392","ENCS-282","ELEC-275","ENGR-301","ELEC-391","ENGR-391","CO-OP","ENGR-371"};
                       var userRoles = e.User.Roles;                // Grabs the list of roles which the user who used this command are a part of.
                       string compare = e.GetArg("role").ToUpper(); // Converts the parameter value to uppercase to avoid upper/lower case mismatch of same strings.


                       if (userRoles.Any(input => input.Name.ToUpper() == "MEMBERS"))   // Checks if user is already registered.
                       {
                           if (compare == roles[0].ToUpper() || compare == roles[1].ToUpper())  // A small secret if anyone tries to get into a role they shouldent.
                           {
                               var singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "System Abuser");
                               await e.User.AddRoles(singleRole);
                               await e.Channel.SendMessage($"Nice try {e.User.Name} but that's not happening... Enjoy this other one however!");
                           }
                           else if (e.GetArg("role") == roles[2])   // Stops users from trying to bypass the register by assigning themselves into the members role.
                           {
                               await e.Channel.SendMessage($"Error: You cannot try to register this way. You must use !sb register");
                           }
                           else
                           {
                               var singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "Members");    // Init. of role variable which will be assigned to user depending on match.
                               switch (compare)                    // Massive Switch statement to determine what user was entered a valid selection or not.
                               {
                                   case "COMP-208":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-208");
                                       pos = 4;                    // Assigning the position of where that role is in the string array.
                                       break;
                                   case "COMP-218":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-218");
                                       pos = 5;
                                       break;
                                   case "COMP-228":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-228");
                                       pos = 6;
                                       break;
                                   case "COMP-232":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-232");
                                       pos = 7;
                                       break;
                                   case "COMP-233":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-233");
                                       pos = 8;
                                       break;
                                   case "COMP-248":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-248");
                                       pos = 9;
                                       break;
                                   case "COMP-249":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-249");
                                       pos = 10;
                                       break;
                                   case "COMP-326":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-326");
                                       pos = 11;
                                       break;
                                   case "COMP-335":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-335");
                                       pos = 12;
                                       break;
                                   case "COMP-339":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-339");
                                       pos = 13;
                                       break;
                                   case "COMP-345":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-345");
                                       pos = 14;
                                       break;
                                   case "COMP-346":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-346");
                                       pos = 15;
                                       break;
                                   case "COMP-348":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-348");
                                       pos = 16;
                                       break;
                                   case "COMP-352":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-352");
                                       pos = 17;
                                       break;
                                   case "COMP-353":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-353");
                                       pos = 18;
                                       break;
                                   case "COMP-354":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-354");
                                       pos = 19;
                                       break;
                                   case "COMP-361":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-361");
                                       pos = 20;
                                       break;
                                   case "COMP-367":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-367");
                                       pos = 21;
                                       break;
                                   case "COMP-371":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-371");
                                       pos = 22;
                                       break;
                                   case "COMP-376":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-376");
                                       pos = 23;
                                       break;
                                   case "COMP-426":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-426");
                                       pos = 24;
                                       break;
                                   case "COMP-428":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-428");
                                       pos = 25;
                                       break;
                                   case "COMP-442":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-442");
                                       pos = 26;
                                       break;
                                   case "COMP-444":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-444");
                                       pos = 27;
                                       break;
                                   case "COMP-445":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-445");
                                       pos = 28;
                                       break;
                                   case "COMP-451":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-451");
                                       pos = 29;
                                       break;
                                   case "COMP-465":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-465");
                                       pos = 30;
                                       break;
                                   case "COMP-472":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-472");
                                       pos = 31;
                                       break;
                                   case "COMP-473":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-473");
                                       pos = 32;
                                       break;
                                   case "COMP-474":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-474");
                                       pos = 33;
                                       break;
                                   case "COMP-476":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-476");
                                       pos = 34;
                                       break;
                                   case "COMP-477":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-477");
                                       pos = 35;
                                       break;
                                   case "COMP-478":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-478");
                                       pos = 36;
                                       break;
                                   case "COMP-479":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-479");
                                       pos = 37;
                                       break;
                                   case "COMP-490":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-490");
                                       pos = 38;
                                       break;
                                   case "COMP-492":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-492");
                                       pos = 39;
                                       break;
                                   case "COMP-495":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-495");
                                       pos = 40;
                                       break;
                                   case "COMP-498":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "COMP-498");
                                       pos = 41;
                                       break;
                                   case "SOEN-228":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-228");
                                       pos = 42;
                                       break;
                                   case "SOEN-287":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-287");
                                       pos = 43;
                                       break;
                                   case "SOEN-298":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-298");
                                       pos = 44;
                                       break;
                                   case "SOEN-321":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-321");
                                       pos = 45;
                                       break;
                                   case "SOEN-331":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-331");
                                       pos = 46;
                                       break;
                                   case "SOEN-341":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-341");
                                       pos = 47;
                                       break;
                                   case "SOEN-342":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-342");
                                       pos = 48;
                                       break;
                                   case "SOEN-343":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-343");
                                       pos = 49;
                                       break;
                                   case "SOEN-344":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-344");
                                       pos = 50;
                                       break;
                                   case "SOEN-345":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-345");
                                       pos = 51;
                                       break;
                                   case "SOEN-357":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-357");
                                       pos = 52;
                                       break;
                                   case "SOEN-384":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-384");
                                       pos = 53;
                                       break;
                                   case "SOEN-385":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-385");
                                       pos = 54;
                                       break;
                                   case "SOEN-387":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-387");
                                       pos = 55;
                                       break;
                                   case "SOEN-390":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-390");
                                       pos = 56;
                                       break;
                                   case "SOEN-422":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-422");
                                       pos = 57;
                                       break;
                                   case "SOEN-423":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-423");
                                       pos = 58;
                                       break;
                                   case "SOEN-448":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-448");
                                       pos = 59;
                                       break;
                                   case "SOEN-449":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-449");
                                       pos = 60;
                                       break;
                                   case "SOEN-487":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-487");
                                       pos = 61;
                                       break;
                                   case "SOEN-490":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-490");
                                       pos = 62;
                                       break;
                                   case "SOEN-491":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-491");
                                       pos = 63;
                                       break;
                                   case "SOEN-498":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "SOEN-498");
                                       pos = 64;
                                       break;
                                   case "ENGR-201":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-201");
                                       pos = 65;
                                       break;
                                   case "ENGR-202":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-202");
                                       pos = 66;
                                       break;
                                   case "ENGR-213":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-213");
                                       pos = 67;
                                       break;
                                   case "ENGR-233":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-233");
                                       pos = 68;
                                       break;
                                   case "ENGR-242":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-242");
                                       pos = 69;
                                       break;
                                   case "ENGR-251":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-251");
                                       pos = 70;
                                       break;
                                   case "ENGR-392":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-392");
                                       pos = 71;
                                       break;
                                   case "ENCS-282":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENCS-282");
                                       pos = 72;
                                       break;
                                   case "ELEC-275":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ELEC-275");
                                       pos = 73;
                                       break;
                                   case "ENGR-301":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-301");
                                       pos = 74;
                                       break;
                                   case "ELEC-391":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ELEC-391");
                                       pos = 75;
                                       break;
                                   case "ENGR-391":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-391");
                                       pos = 76;
                                       break;
                                   case "CO-OP":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "CO-OP");
                                       pos = 77;
                                       break;
                                   case "ENGR-371":
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "ENGR-371");
                                       pos = 78;
                                       break;
                                   default:
                                       singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "");   // If no match happens it gives a null role.
                                       pos = 3;
                                       await e.Channel.SendMessage($"Error: Group does not exist or was entered incorrectly.");
                                       break;
                               }
                               await e.User.AddRoles(singleRole);   // Applies the role matched to the user who sent the message, if no match then a null role is assigned which effectively does nothing and stops executing.
                               await e.Channel.SendMessage($"{e.User.Name} has been successfully added to group {roles[pos]}"); // Confirmation of role assignment.
                           }
                       }
                       else
                       {
                           await e.Channel.SendMessage($"Error: {e.User.Name} is not registered. To register please use !sb register.");
                       }
                   });

                ins.CreateCommand("register")   // Command to register users.
                  .Description("Allows a user to register to server.")
                  .Do(async (e) =>
                  {
                      var userRoles = e.User.Roles;

                      if (userRoles.Any(input => input.Name.ToUpper() == "MEMBERS"))
                      {
                          await e.Channel.SendMessage($"{e.User.Name} is already registered!");
                      }
                      else
                      {
                          var singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == "Members");
                          await e.User.AddRoles(singleRole);
                          await e.Channel.SendMessage($"{e.User.Name} has been successfully registered!");
                      }   
                  });

                ins.CreateCommand("leave")      // Command to let users leave a role.
                  .Description("Allows a user to leave a role.")
                  .Parameter("role", ParameterType.Required)
                  .Do(async (e) =>
                  {
                      string currentRole = e.GetArg("role").ToUpper();
                      var userRoles = e.User.Roles;

                      if (userRoles.Any(input => input.Name.ToUpper() == currentRole))
                      {
                          var singleRole = e.Server.Roles.FirstOrDefault(x => x.Name == currentRole);
                          await e.User.RemoveRoles(singleRole);
                          await e.Channel.SendMessage($"{e.User.Name} has been successfully removed from group {currentRole}!");
                      }
                      else
                      {
                          await e.Channel.SendMessage($"{e.User.Name} has already left {currentRole}!");
                      }
                  });

                ins.CreateCommand("list")       // Command to list all available classes.
                  .Description("Lists all classes.")
                  .Do(async (e) =>
                  {
                      string compCourses = "COMP: 108, 201, 208, 218, 228, 232, 233, 248, 249, 326, 335, 339, 345, 346, 348, 352, 353, 354, 361, 367, 371, 376, 426, 428, 442, 444, 445, 451, 465, 472, 473, 474, 476, 477, 478, 479, 490, 492, 495, 498",
                      soenCourses = "SOEN: 228, 287, 321, 331, 341, 342, 343, 344, 345, 357, 384, 385, 387, 390, 422, 423, 448, 449, 487, 490, 491, 498",
                      engrCourses = "ENGR: 201, 202, 213, 233, 242, 251, 301, 391, 392, 392",
                      encsCourses = "ENCS: 282",
                      elecCourses = "ELEC: 275";
                      await e.Channel.SendMessage($"Here are the lists of all courses:\n {soenCourses} \n {compCourses} \n {engrCourses} \n {encsCourses} \n {elecCourses}");
                  });


                ins.CreateCommand("test")       // Hidden text command. Used only for when things REALLY go wrong.
                    .Description("Test. Syntax: !sb test")
                    .Do(async (e) =>
                    {
                        await e.Channel.SendMessage("A test echo command at end.");
                    });
            });
        }

        public void Log(object sender, LogMessageEventArgs e)   // Log handler to console.
        {
            Console.WriteLine($"[{e.Severity}] {e.Source}: {e.Message}");   // Writes console line with information based on events.
        }
    }
}
