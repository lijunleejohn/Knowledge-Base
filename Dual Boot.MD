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
  
  
