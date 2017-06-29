# Windows Server Firewall Setup
Sometimes images speak louder than words, so we've compiled a list of images to show you exactly what you need to do in order to allow the Master Server to work on your Windows Server either in your home or in the cloud!

First you need to open your **Advanced Firewall Settings"

![windows-server-master-server-1](/images/windows-server-master-server-1.png "Windows Server Master Server 1")

Next you need to select **Inbound Rules** then select **New Rule...**

![windows-server-master-server-1](/images/windows-server-master-server-2.png "Windows Server Master Server 2")

Next you will need to selec **Port** and then click **Next**

![windows-server-master-server-1](/images/windows-server-master-server-3.png "Windows Server Master Server 3")

Next, make sure that **TCP** is selected and that you enter the correct port number for your Master Server. The default port is **15940**. Then click **Next**

![windows-server-master-server-1](/images/windows-server-master-server-4.png "Windows Server Master Server 4")

Next you need to select **Allow the connection**. Click **Next**

![windows-server-master-server-1](/images/windows-server-master-server-5.png "Windows Server Master Server 5")

After that, just keep all the checkboxes at default (cheched). Click **Next**

![windows-server-master-server-1](/images/windows-server-master-server-6.png "Windows Server Master Server 6")

Lastly give it a good name that is easy to find and click **Finish**

![windows-server-master-server-1](/images/windows-server-master-server-7.png "Windows Server Master Server 7")

## Done
That is it, you should be ready to go. If you experience any other issues, make sure that your network firewall is allowing the connection to this machine and allowing the specified port number.