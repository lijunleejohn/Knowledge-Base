#Dual Boot Linux and Windows
* Add Windows Boot Manager to Grub
  * sudo os-prober
  * sudo update-grub
* Fix Windows Boot Manager overrides Grub boot, from a Windows Command Prompt window launched with Administrator privileges:
  * Run bcdedit command to view the original setting
  * For Zorin: bcdedit /set {bootmgr} path \EFI\Boot\grubx64.efi
  * For Ubuntubcdedit /set {bootmgr} path \EFI\ubuntu\grubx64.efi
  * Run bcdedit again to view the changes
* List hard drive partitions
  * sudo fdisk -l
* Use "boot-repair" tool, it comes with the Zorin Boot USB, reference URL: https://askubuntu.com/questions/661947/add-windows-10-to-grub-os-list
 * <p>use this app EasyBCD for windows http://neosmart.net/EasyBCD/ download the free version and install it.

when you open it you will see an entry for windows 10 and one for windows 10.

delete the windows 8 entry and you'll be good to go.

Update after doing the steps above go back to Ubuntu,

open the terminal by pressing ctrl+alt+T

then copy past this commands to it one after another,

sudo add-apt-repository ppa:yannubuntu/boot-repair
sudo apt-get update
sudo apt-get install -y boot-repair
 
and by this you will install a tool called boot-repair.

open boot repair from dash and you'll get this window

enter image description here

choose the recommended repair and follow the steps</p>
  
  
