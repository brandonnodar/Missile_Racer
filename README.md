<h1 align="center">
  <br>
  <img src="https://raw.githubusercontent.com/brandonnodar/Missile_Racer/master/images/mr_title.gif" alt="Missile Racer" width="600">
  </br>
  Missile Racer
</h1>
<br>
<p align="center">This project was made in Unity3D, written in C#. This is my biggest project to date, 1.5 years to complete.</p>

<p align="center">
  <a href="#about-the-project">About the Project</a> •
  <a href="#ai-missile">AI Missile</a> •
  <a href="#online-leaderboards">Online Leaderboards</a> •
  <a href="#missile-garage">Missile Garage</a> •
  <a href="#game-modes">Game Modes</a> •
  <a href="#notes">Notes</a>
</p>

## About the Project
<p>The purpose of the game is to be the first missile to hit the enemy on the other side</p>

<p>(Add gif of missiles about to hit enemy)</p>

## AI Missile
<p>The AI has been programmed to follow a set of waypoints. The waypoints adjust based on the AI's missile performance (how fast it can go), and the difficulty (how closely the AI takes the turns).</p>

<p>(Add gif of AI taking turn (in scene view within Unity, showing waypoints and triggers).

## Online Leaderboards
<p>Compete in online leaderboards for the fastest time on the track.</p>
<ul>
  <li>Uses Microsoft's Playfab API to send and recieve times.</li>
  <li>New track every 24 hours.</li>
  <li>Integrated user logins to save times to the Playfab API.</li>
</ul>

## Missile Garage
<p>The garage allows you to change, upgrade, and customize your missile.</p>
<p>Features of the garage include:</p>
<ul>
  <li>30 missiles to choose from.</li>
  <li>4 performance upgrades.</li>
  <li>5 paint jobs for each missile.</li>
  <li>130 different thrusters and explosion styles.</li>
</ul>

<p>Each missile saves it's state of selected upgrade, body color, thruster, and explosion settings.</p>

## Game Modes
<p>There are a variety of game modes, and mini games you can play.</p>
<ul>
  <li>Race: Compete against an AI, first to hit the enemy wins.</li>
  <li>Time Trials: Beat the time. Taking too long self destructs your missile.</li>
  <li>Defender: Hit your opponent's missile with your missile before your opponent hits the enemy.</li>
  <li>Capture The Flag: Capture more flags than your opponent.</li>
  <li>Slalom: Fly between the gates in a certain amount of time.</li>
</ul>

<p>There are also different ways to play them:</p>
<ul>
  <li>Career: Play through 5 countries, racing on all 30 tracks.</li>
  <li>Tournament: Race on 5 random tracks, with only 3 chances to beat each one.</li>
  <li>Olympia: </li>
</ul>

<p>There are also mini games!</p>
<ul>
  <li>Hoops: Use your missile to hit the basketball into the hoop. Score the most points in 60 seconds.</li>
  <li>Pizza Run: Collect the pizza from the other side, and bring it back before time runs out.</li>
  <li>Ninja Run: Fly through tight turns, avoid homing missiles, and dodge bullets to the finish line.</li>
</ul>
 
## Notes
<p>Not all scripts are included in this repo. I've selected a few key scripts to give an idea of the mechanics of the game and system.</p>
