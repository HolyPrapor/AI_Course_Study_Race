const canvas = document.getElementById("field");
const con = document.getElementById("console");
const ctx = canvas.getContext('2d');

let cellSize = 8;
let index = 0;
let log = null;

let firebaseConfig = {
    apiKey: "AIzaSyBO8QoDZknDF3XY1dw_tXeIVbPsvDiBIdw",
    authDomain: "game-ai-logs.firebaseapp.com",
    databaseURL: "https://game-ai-logs.firebaseio.com",
    projectId: "game-ai-logs",
    storageBucket: "game-ai-logs.appspot.com",
    messagingSenderId: "655269759371",
    appId: "1:655269759371:web:b8baab1eebefaf2617078e"
};
firebase.initializeApp(firebaseConfig);
const db = firebase.firestore();
const url = new URL(document.location.href);
document.body.onkeydown = keydown;
canvas.onclick = click;

firebase.auth().onAuthStateChanged(function(user) {
    loadApp().catch(console.log);
});

function handleFirebaseError(e){
    console.log(e);
    console.log("Sign out");
    firebase.auth().signOut();
}

async function loadApp(){
    let logId = url.searchParams.get("logId");
    if (logId){
        originalLog = null;
        await auth();
        try {
            const doc = await db.collection("logs").doc(logId).get();
            if (doc.exists) {
                const data = doc.data();
                log = convertGameLog(JSON.parse(data.log));
                document.getElementById("replayCreationTime").innerText = data.creationTime;
                document.getElementById("replayAuthor").innerText = data.authorEmail;
                update();
            }
        }
        catch(e) {
            handleFirebaseError(e);
        }
    }
    else {
        log = convertGameLog(originalLog);
        update();
        document.getElementById("sharelink").style.display = "block";
    }
}

function auth(){
    console.log(firebase.auth().currentUser);
    if (firebase.auth().currentUser) return;
    var provider = new firebase.auth.GoogleAuthProvider();
    return firebase.auth().signInWithRedirect(provider);    
}

async function shareLink(){
    await auth();
    const logData = JSON.stringify(originalLog);
    const logId = md5(logData);
    try {
        await db.collection("logs").doc(logId).set({
            log: logData,
            creationTime: new Date().toString(),
            authorEmail: firebase.auth().currentUser.email
        });
    } 
    catch (e){
        handleFirebaseError(e);
        return;
    }
    const url = new URL("https://icfpc2020-logs.firebaseapp.com/");
    url.search = "logId=" + logId;
    const link = document.getElementById("link");
    link.innerText = link.href = url.href;
    document.getElementById("sharelink").style.display = "none";
}

function keydown(ev){
    const code = ev.code;
    console.log(code);
    if (code === "ArrowRight" || code === "KeyD")
        rewind(1);
    else if (code === "ArrowLeft" || code === "KeyA")
        rewind(-1);
    else if (code === "Home" || code === "KeyQ")
        index = 0;
    else if (code === "End" || code === "KeyE")
        index = log.ticks.length-1;
    else return;
    ev.preventDefault();
    update();
}

function click(ev){
    console.log(ev); 
    update();
}

function convertGameLog(data) {
    return {
        raceDuration: data[0],
        flags: data[1],
        obstacles: data[2],
        ticks: data[3].map(convertTick)
    };
}

function convertTick(data){
    return {
        time: data[0],
        isFinished: data[1],
        firstCar: convertCar(data[2]),
        secondCar: convertCar(data[3]),
    };
}

function convertCar(data) {
    return {
        pos: data[0],
        v: data[1],
        radius: data[2],
        flagsTaken: data[3],
        isAlive: data[4],
        nextCommand: data[5],
        debugOutput: data[6],
        debugLines: data[7].map(convertLine)
    };
}

function convertLine(data){
    return {
        start: data[0],
        end: data[1],
        intensity: data[2]
    };
}

function rewind(dir){
    index = Math.min(log.ticks.length-1, Math.max(0, index+dir));
}

function update() {
    drawEvent(log.ticks[index]);
}

function drawEvent(tick){
    const firstCar = tick.firstCar;
    const secondCar = tick.secondCar;
    adjustScale(firstCar);
    adjustScale(secondCar);
    clearSpace();
    drawFlags(log.flags, firstCar, secondCar);
    drawObstacles(log.obstacles);
    consoleOut = "Tick: " + tick.time + "\n";
    consoleOut += "Debug info: " + firstCar.debugOutput + "\n";
    drawCar(firstCar, "First car: ");
    drawCar(secondCar, "Second car: ");
    con.innerText = consoleOut;
}

function replacer(key,value)
{
    if (key=="debugLines") return undefined;
    else if (key=="debugOutput") return undefined;
    else return value;
}

function drawFlags(flags, firstCar, secondCar){
    for(let i = 0; i < flags.length; i++){
        let isNext = i == (firstCar.flagsTaken + secondCar.flagsTaken) % flags.length;
        drawFlag(flags[i], i, isNext);
    }
}

function drawObstacles(obstacles){
    for(let i = 0; i < obstacles.length; i++){
        drawObstacle(obstacles[i]);
    }
}

function drawObstacle(o){
    ctx.fillStyle = 'grey';
    ctx.fill(createDisk(...o));
}

function drawFlag(flag, index, isNext) {
    ctx.fillStyle = isNext ? 'cyan' : 'blue';
    ctx.fill(createDisk(flag[0], flag[1]));
}

function drawCar(car, prependation) {
    const pos = car.pos;
    ctx.fillStyle = 'green';
    ctx.fill(createDisk(pos[0], pos[1], car.radius));
    for(let line of car.debugLines){
        ctx.strokeStyle = "rgba(255,165,0," + line.intensity + ")";
        ctx.stroke(createLine(line));
    }
    consoleOut += prependation + JSON.stringify(car, replacer) + "\n";
}

function createLine(line){
    const res = new Path2D();
    res.moveTo(toScreenX(line.start[0]), toScreenY(line.start[1]));
    res.lineTo(toScreenX(line.end[0]), toScreenY(line.end[1]));
    return res; 
}

function clearSpace() {
    ctx.clearRect(0,0,canvas.width, canvas.height);
}

function toScreenX(gameX){
    const originX = canvas.width / 2 - cellSize / 2;
    return originX + gameX*cellSize;
}

function toScreenY(gameY){
    const originY = canvas.height / 2 - cellSize / 2;
    return originY + gameY*cellSize;
}

function createDisk(gameX, gameY, radius = 0) {
    const res = new Path2D();
    let x = toScreenX(gameX);
    let y = toScreenY(gameY);
    let size = radius*cellSize;
    if (size < 4)
        size = 4;
    res.arc(x, y, size, 0, 2*Math.PI);
    return res;
}

function createRect(gameX, gameY, radius = 0) {
    const res = new Path2D();
    let size = (2*radius+1)*cellSize;
    let left = toScreenX(gameX-radius);
    let top = toScreenY(gameY-radius);
    const minSize = 4;
    if (size < minSize){
        left -= (minSize-size)/2;
        top -= (minSize-size)/2;
        size = minSize;
    }
    res.rect(left, top, size, size);
    return res;
}

function fitInScreen(pos){
    const screenX = toScreenX(pos[0]);   
    const screenY = toScreenY(pos[1]);
    return screenX > 0 && screenX < canvas.width && screenY > 0 && screenY < canvas.height;
}

function adjustScale(car) {
    let objects = [car.pos, ...log.flags, ...log.obstacles];
    //cellSize = 8;
    while (cellSize > 0.2 && objects.some(p => !fitInScreen(p)))
        cellSize/=1.2;
}