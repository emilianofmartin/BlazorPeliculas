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
        timer = setTimeout(logout, 5 * 1000); // 5 minutos
    }

    function logout() {
        dotnetHelper.invokeMethodAsync("Logout");
    }
}