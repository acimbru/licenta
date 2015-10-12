// Attach this script to an existing particle system.

function LateUpdate () {
// extract the particles
var particles = particleEmitter.particles;
for (var i = 0; i < particles.Length; i++) {
// Move the particles up and down on a sinus curve
var xPosition = Mathf.Sin (Time.time*1.5) * Time.deltaTime;
var yPosition = Mathf.Sin (Time.time*2) * Time.deltaTime;
var zPosition = Mathf.Sin (Time.time*2.5) * Time.deltaTime;

particles[i].position += Vector3 (xPosition+(Mathf.Cos(particles[i].position.x*5))/150, yPosition+(Mathf.Cos(particles[i].position.z*5))/100, zPosition+(Mathf.Cos(particles[i].position.y*5))/100);
// make the particles red
particles[i].color = Color.red;
// modify the size on a sinus curve
//particles[i].size = Mathf.Sin (Time.time) * 0.5;
}
// copy them back to the particle system
particleEmitter.particles = particles;
}