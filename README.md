# Probabilistisk Modellering
* Download the program from https://github.com/Shizukanawa/ProbabilistiskModellering/releases/latest

# WARNING!! RUNNING THE PROGRAM MAY CAUSE YOUR COMPUTER TO BE SLOW/UNRESPONSIVE

## Requirements
* SUMO 1.2.0
* .NET Framework 4.6.1

## Sumo CLI
1. Compile the program
2. Open CMD into the SUMOFiles folder
3. You have 2 choices:
  - Type `sumo --remote-port (port number) -c cfg.sumocfg -W true` into cmd. Make sure to allow access to the firewall prompt and close the command prompt.
  - Add Sumo CLI manually by allowing it through the firewall.
5. Now run the program and enter population count, gene length, what generation number to stop at and what fitness number the program should stop at.
6. Depending on the amount of population count, your computer may be slow.
