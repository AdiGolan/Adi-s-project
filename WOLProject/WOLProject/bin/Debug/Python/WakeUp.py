#Region - imports
import socket
import struct
from scapy.all import *
#EndRegion - imports

#Region - Constants
MAC_BROADCAST="ff:ff:ff:ff:ff:ff"

DST_PORT=9

DST_IP="255.255.255.255"

ADDR= (DST_IP, DST_PORT)

#EndRegion - Constants

class WakeUp():

    def __init__(self,mac_addr,option):
        self.mac_addr = mac_addr
        self.Fix()
        self.option = option

    def Fix(self):

        if len(self.mac_addr) == 12:
            pass

        elif len(self.mac_addr) == 12 + 5:
            self.mac_addr = self.mac_addr.replace(self.mac_addr[2], '')
            print 'SW'
        else:
            raise ValueError('Incorrect MAC address format')
        
    def Run(self): #Sends a magic Packet in scapy
        
        if self.option == 1:  #Socket
            dot = MAC_BROADCAST[2]
            mac_broadcast = MAC_BROADCAST.replace(dot,'')
            mac_addr = self.mac_addr
            MAC_ADDR = mac_addr
            data = ''.join([mac_broadcast,MAC_ADDR*20])
            send_to =''
            for i in range(0, len(data), 2):
                send_to += struct.pack('B',int(data[i:i+2],16))
            sock = socket.socket(socket.AF_INET, socket.SOCK_DGRAM)
            sock.setsockopt(socket.SOL_SOCKET, socket.SO_BROADCAST, 1)
            sock.sendto("shalom", ('10.92.5.27', 7))
            for i in xrange(3):
                sock.sendto(send_to, ADDR)
                
        if self.option == 2: #Scapy

            MAC_ADDR=self.mac_addr.decode("hex")

            mac_broadcast=MAC_BROADCAST.replace(MAC_BROADCAST[2], "").decode("hex")
            MagicPacket=Ether(dst=MAC_BROADCAST)/IP(dst=DST_IP)/UDP(dport=DST_PORT)/Raw(mac_broadcast+16*MAC_ADDR)

            for i in range(3):

                sendp(MagicPacket)
        else:
            #raise "Error - Options Can Be Only 1 or 2"
            pass
        

if __name__ == "__main__":
    testunit = WakeUp("6C-3B-E5-24-02-6F",2)
    testunit.Run()

