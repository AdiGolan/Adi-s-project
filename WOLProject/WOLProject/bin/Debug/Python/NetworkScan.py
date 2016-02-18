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
GUI_PORT = 12345
#endregion

#region ----------   CLASSES   -----------------------------
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
    except socket.error , e:
            print e
    

#endregion

if __name__ == "__main__":
    main()

