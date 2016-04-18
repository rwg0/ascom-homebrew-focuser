# ascom-homebrew-focuser

Source code for a simple ASCOM Focuser driver that can be used with Homebrew USB/Serial adaptors as 
discussed on http://www.astronomyshed.co.uk/forum/viewforum.php?f=63

In essence the hardware communication is via a serial port (real or USB) and the focuser moves
when data is sent to the port and direction is controlled via the RTS line.