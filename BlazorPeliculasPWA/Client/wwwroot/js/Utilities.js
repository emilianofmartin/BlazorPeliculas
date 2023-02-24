function DotNetStaticTest() {
    DotNet.invokeMethodAsync("BlazorPeliculas.Client", "GetCurrentCount")
        .then(result => {
            console.log('Count from js: ' + result);
        });
}

function DotNetInstanceTest(dotNetHelper) {
    dotNetHelper.invokeMethodAsync("IncrementCount");
}

function inactiveTimer(dotnetHelper) {
    var timer;

    document.onmousemove = resetimer;
    document.onkeypress = resetimer;

    function resetimer() {
        clearTimeout(timer);
        timer = setTimeout(logout, 5 * 60 * 1000); // 5 minutos
    }

    function logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }
}

var db = new Dexie("mydb");
var dbVersion = 1;

db.version(dbVersion).stores({
    createStore: 'id++',
    deleteStore: 'id++'
});

async function getPendingRecords() {
    return await {
        ObjectsToCreate: await db.createStore.toArray(),
        ObjectsToDelete: await db.deleteStore.toArray()
    };
}

async function deleteRecord(table, id) {
    await db[table].where({ "id": id }).delete();
}

async function getPendingRecordCount() {
    var count = await db.createStore.count();
    count += await db.deleteStore.count();

    return count;
}

async function saveCreateRecord(url, body) {
    await db.createStore.put({ url, boby: JSON.parse(body) });
}

async function saveDeleteRecord(url) {
    await db.deleteStore.put({ url });
}

async function getStatusNotificationGrant() {
    const grant = Notification.permission;

    if(grant === 'denied') return grant;

    //Estamos suscriptos?
    const worker = await navigator.serviceWorker.getRegistration();
    const existingSuscription = await worker.pushManager.getSubscription();

    if(existingSuscription)
        return "granted";
    else
        return "default";   //Aún no contestó
}

async function suscribeUser() {
    var notificationGrant = await Notification.requestPermission();
    if (notificationGrant != 'granted') return null;

    const worker = await navigator.serviceWorker.getRegistration();
    const existingSuscription = await worker.pushManager.getSubscription();

    if (!existingSuscription) {
        const publicKeyResponse = await fetch('/api/config/noficationspublickey');
        const publicKey = await publicKeyResponse.text();

        const newSubscription = await worker.pushManager.subscribe({
            userVisibleOnly: true,
            applicationServerKey: publicKey
        });

        return buildSuscriptionResponse(newSubscription);
    }
    else {
        return buildSuscriptionResponse(existingSuscription);
    }
}

async function unsuscribeUser() {
    const worker = await navigator.serviceWorker.getRegistration();
    const existingSuscription = await worker.pushManager.getSubscription();

    if(existingSuscription) {
        existingSuscription.unsubscribe();
        return buildSuscriptionResponse(existingSuscription);
    }
}

function buildSuscriptionResponse(suscription) {
    return {
        url: suscription.endpoint,
        p256dh: arrayBufferToBase64(suscription.getKey('p256dh')),
        auth: arrayBufferToBase64(suscription.getKey('auth'))
    };
}

function arrayBufferToBase64(buffer) {
    // https://stackoverflow.com/a/9458996
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}