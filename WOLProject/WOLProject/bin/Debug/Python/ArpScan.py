from scapy.all import *
conf.verb = 0

class ArpScan():
    def __init__(self, FinalGateway):
        self.connList = []
        self.FinalGateway = FinalGateway
    def Scan(self):
        pkt = Ether(dst="ff:ff:ff:ff:ff:ff") / ARP(pdst = self.FinalGateway)
        ans, uans = srp(pkt, timeout=2)
        print len(ans)
        for s,r in ans:
            self.connList.append( (r[ARP].psrc, r[ARP].hwsrc) )
        return self.connList


connList = ArpScan("10.92.4.0/22").Scan() 
for con in connList:
    print con
