using System;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using Chat = System.Net;
using System.IO;


namespace komunikator
{
    class Server
    {
        TcpListener chatServer; // nasłuchwianie połączeń od klientów tcp
        public static Hashtable nickName;
        public static Hashtable nickNameByConnect;

        public Server()
        {
            nickName = new Hashtable(100);
            nickNameByConnect = new Hashtable(100);
            chatServer = new TcpListener(4296);

            while (true)
            {
                chatServer.Start(); //rozpoczecie nasluchwania

                if (chatServer.Pending()) //okresla czy zadania oczekujace polaczenia
                {

                    TcpClient chatConnection = chatServer.AcceptTcpClient();
                    Console.WriteLine("Jesteś połączony :) ");
                    DoCommunicate comm = new DoCommunicate(chatConnection);
                    
                }
            }
        }


        public static void SendMessToAll(string nick, string mess)
        {
            StreamWriter writer; //uzywany do zapisu wiadomosci do okna czatu
            ArrayList toRemowe = new ArrayList(0);

            TcpClient[] tcpClient = new TcpClient[Server.nickName.Count]; //pomieszczenie wszystkich tcp polaczonych uzytkownikow
            Server.nickName.Values.CopyTo(tcpClient, 0);

            for (int c = 0; c < tcpClient.Length; c++)
            {
                try
                {
                    if (mess.Trim() == "" || tcpClient[c] == null) //czy wiadomosc jest pusta i 
                                                                     //czy indeks tablicy jest pusty
                        continue;

                    writer = new StreamWriter(tcpClient[c].GetStream());
                    writer.WriteLine(nick + ". " + mess);
                    writer.Flush(); //czyści wszystkie bufory tego strumienia i powoduje, 
                    //że wszystkie buforowane dane są zapisywane w odpowiednie urządzenia.
                    writer = null;
                }
                catch (Exception e44) //opuszczenie lub rozlaczenie uzytkownika
                {
                    e44 = e44;
                    string str = (string)Server.nickNameByConnect[tcpClient[c]];
                    Server.SendSystemMess("** " + str + " ** Opuscil chat");
                    Server.nickName.Remove(str); //usuwanie nazwy z listy
                    Server.nickNameByConnect.Remove(tcpClient[c]); //usuwamy instancje uzytkownika
                }
            }
        }
        public static void SendSystemMess(string mess) //wiadomosci wysylane przez system
        {
            StreamWriter writer; //uzywany do zapisu wiadomosci do okna czatu
            ArrayList toRemowe = new ArrayList(0);

            TcpClient[] tcpClient = new TcpClient[Server.nickName.Count]; //pomieszczenie wszystkich tcp polaczonych uzytkownikow
            Server.nickName.Values.CopyTo(tcpClient, 0);

            for (int i = 0; i < tcpClient.Length; i++)
            {
                try
                {
                    if (mess.Trim() == "" || tcpClient[i] == null) //czy wiadomosc jest pusta i 
                                                                     //czy indeks tablicy jest pusty
                        continue;

                    writer = new StreamWriter(tcpClient[i].GetStream());
                    writer.WriteLine(mess);
                    writer.Flush(); //czyści wszystkie bufory tego strumienia i powoduje, 
                                    //że wszystkie buforowane dane są zapisywane w odpowiednie urządzenia.
                    writer = null;
                }
                catch (Exception e44) //opuszczenie lub rozlaczenie uzytkownika
                {
                    e44 = e44;
                   
                    Server.nickName.Remove(Server.nickNameByConnect[tcpClient[i]]); //usuwanie nazwy z listy
                    Server.nickNameByConnect.Remove(tcpClient[i]); //usuwamy instancje uzytkownika
                }
            }

        }
    
    
    
    
    }

   
}
