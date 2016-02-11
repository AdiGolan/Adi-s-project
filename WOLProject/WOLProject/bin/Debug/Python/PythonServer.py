#region ----------   ABOUT   -----------------------------
"""
##################################################################
# Created By:  Adi Golan                                         #
# Date: 20/09/2014                                               #
# Name: Server  between GUI and clients                          #
# Version: 1.0                                                   #
# Windows Tested Versions: Win 7 32-bit                          #
# Python Tested Versions: 2.6 32-bit                             #
# Python Environment  : PyCharm                                  #
##################################################################
"""
#endregion

#region ----------   IMPORTS   -----------------------------
import threading,socket, sys, os
#endregion


#region -----  CONSTANTS  -----
# For every client to been thread
THREAD_LIMIT = 50
GUI_PORT = 12345
CLIENT_PORT = 2000
SERVER_ABORT = "Aborting the server..."
#endregion

#region ----------   CLASSES   -----------------------------
#region -----  PythonServer CLASS  -----
class  PythonServer(threading.Thread):   
    # -----  DATA  -----
    listenerSock = None
    # Dictionary for client connctions : Key - ip  Value - SessionWithClient
    open_clients = {}       
 
    # constructor 
    def __init__(self, gui):
        self.gui = gui
        threading.Thread.__init__(self)
                
    # the main thread function
    def  run(self):
        from SessionWithClient import *
        self.gui.guiSock.send("Server running...Waiting for a connection...#")   # to GUI 
        try:
            # Listener socket
            listenerSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
            listenerSock.bind(("",CLIENT_PORT))
            listenerSock.listen(5)
            self.gui.guiSock.send("Listening to clients...#")   # to GUI 

            while True:
                clientSock, addr = listenerSock.accept()                 
                # Thread creating loop
                while True:
                    if threading.activeCount() < THREAD_LIMIT:
                        clientIP = addr[0]  # key - IP client
                        # Check if client connected in pass
                        if clientIP in self.open_clients:
                            # Callback Connection by IP clientIP
                            self.gui.guiSock.send("Callback Connection by " +  clientIP + " clientIP#")   # to GUI
                        else:
                            # First Connection by IP  clientIP
                            sessionWithClient = SessionWithClient(self, clientSock, addr )
                            sessionWithClient.start()
                        break
        except socket.error , er_msg:
            error_code = er_msg[0]
            if error_code == 10048:
                self.gui.guiSock.send("Port " + str(CLIENT_PORT) + " is busy#")   # to GUI  
            else:
                self.gui.guiSock.send(str(er_msg) + "#")   # to GUI            
        except Exception as er_msg:
            self.gui.guiSock.send(str(er_msg) + "#")   # to GUI  

#endregion

#region ----------    Network Scan
from scapy.all import *
conf.verb = 0

class ArpScan():
    def __init__(self, FinalGateway):
        self.connList = []
        self.FinalGateway = FinalGateway

    def Scan(self):
        pkt = Ether(dst="ff:ff:ff:ff:ff:ff") / ARP(pdst = self.FinalGateway)
        ans, uans = srp(pkt, timeout=2)
        for s,r in ans:
            self.connList.append( (r[ARP].psrc, r[ARP].hwsrc) )
        mes = ""   
        for con in self.connList:
            mes += con[0] + "@" + con[1] + "#"
        return mes


#region -----  CLASS  GUI  -----
class  Gui(threading.Thread):   
    pythonServer = None
    # constructor 
    def __init__(self):
        # socket between the this server and the GUI
        self.guiSock = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
        self.guiSock.connect(("127.0.0.1", GUI_PORT))
        threading.Thread.__init__(self)
                
    # the main function
    def  run(self):
        connList = ArpScan("10.92.5.0/24").Scan() 
        self.guiSock.send("ARP#" + connList)

        while True:
            # Wait message from GUI 
            data = self.guiSock.recv(1024)
            if len(data) > 0 :
                pass
                 
#endregion
#endregion


#region ----------   MAIN   -----------------------------

def main():
    """
    Description: Main function, connect to target server, establish protocol,
    """
    try:
        gui = Gui()            #  connection to GUI process
        gui.start()            #  start thtread loop for session with GUI         
        # construct server for sessions with clients     
        gui.pythonServer = PythonServer(gui) 
        gui.pythonServer.start() 
    except socket.error , e:
            print e
    

#endregion

if __name__ == "__main__":
    main()

