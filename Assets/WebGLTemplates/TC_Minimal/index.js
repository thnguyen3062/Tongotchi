window.addEventListener("load", function () {
  if ("serviceWorker" in navigator) {
    navigator.serviceWorker.register("ServiceWorker.js");
  }
});
var unityInstanceRef;
var unsubscribe;
var container = document.querySelector("#unity-container");
var canvas = document.querySelector("#unity-canvas");
var loadingBar = document.querySelector("#unity-loading-bar");
var progressBarFull = document.querySelector("#unity-progress-bar-full");
var warningBanner = document.querySelector("#unity-warning");

// Shows a temporary message banner/ribbon for a few seconds, or
// a permanent error message on top of the canvas if type=='error'.
// If type=='warning', a yellow highlight color is used.
// Modify or remove this function to customize the visually presented
// way that non-critical warnings and error messages are presented to the
// user.
function unityShowBanner(msg, type) {
  function updateBannerVisibility() {
    warningBanner.style.display = warningBanner.children.length ? 'block' : 'none';
  }
  var div = document.createElement('div');
  div.innerHTML = msg;
  warningBanner.appendChild(div);
  if (type == 'error') div.style = 'background: red; padding: 10px;';
  else {
    if (type == 'warning') div.style = 'background: yellow; padding: 10px;';
    setTimeout(function () {
      warningBanner.removeChild(div);
      updateBannerVisibility();
    }, 5000);
  }
  updateBannerVisibility();
}

var buildUrl = "Build";
var loaderUrl = buildUrl + "/{{{ LOADER_FILENAME }}}";
var config = {
  dataUrl: buildUrl + "/{{{ DATA_FILENAME }}}",
  frameworkUrl: buildUrl + "/{{{ FRAMEWORK_FILENAME }}}",
  #if USE_THREADS
    workerUrl: buildUrl + "/{{{ WORKER_FILENAME }}}",
  #endif
#if USE_WASM
    codeUrl: buildUrl + "/{{{ CODE_FILENAME }}}",
  #endif
#if MEMORY_FILENAME
    memoryUrl: buildUrl + "/{{{ MEMORY_FILENAME }}}",
  #endif
#if SYMBOLS_FILENAME
    symbolsUrl: buildUrl + "/{{{ SYMBOLS_FILENAME }}}",
  #endif
    streamingAssetsUrl: "StreamingAssets",
  companyName: {{{ JSON.stringify(COMPANY_NAME) }}},
productName: "Tongotchi",
  productVersion: 2,
    showBanner: unityShowBanner,
  };

// By default Unity keeps WebGL canvas render target size matched with
// the DOM size of the canvas element (scaled by window.devicePixelRatio)
// Set this to false if you want to decouple this synchronization from
// happening inside the engine, and you would instead like to size up
// the canvas DOM size and WebGL render target sizes yourself.
// config.matchWebGLToCanvasSize = false;

if (/iPhone|iPad|iPod|Android/i.test(navigator.userAgent)) {
  // Mobile device style: fill the whole browser client area with the game canvas:
  var meta = document.createElement('meta');
  meta.name = 'viewport';
  meta.content = 'width=device-width, height=device-height, initial-scale=1.0, user-scalable=no, shrink-to-fit=yes';
  document.getElementsByTagName('head')[0].appendChild(meta);
}

#if BACKGROUND_FILENAME
canvas.style.background = "url('" + buildUrl + "/{{{ BACKGROUND_FILENAME.replace(/'/g, '%27') }}}') center / cover";
#endif
loadingBar.style.display = "block";

var script = document.createElement("script");
script.src = loaderUrl;
script.onload = () => {
  createUnityInstance(canvas, config, (progress) => {
    progressBarFull.style.width = 100 * progress + "%";
  }).then((unityInstance) => {
    unityInstanceRef = unityInstance;

    loadingBar.style.display = "none";
    function getUser() {
      Telegram.WebApp.ready();
      // Function to get query parameter value by name
      function getQueryParam(name) {
        const urlParams = new URLSearchParams(window.location.search);
        return urlParams.get(name);
      }

      // Get the tgWebAppStartParam parameter from the URL
      const startParam = getQueryParam('tgWebAppStartParam');

      // Prepare the data to be sent to the server
      const postData = {
        query: Telegram.WebApp.initData,
        command: startParam // Set command as empty string as specified
      };

      unityInstance.SendMessage('LoadingScene', 'ReceiveUserID', JSON.stringify(postData));

      window.addEventListener('load', function () {
        Telegram.WebApp.ready();
        Telegram.WebApp.expand();

        console.log("Telegram web app has been expanded to full screen");
      });
      Telegram.WebApp.onEvent('invoiceClosed', HandleInvoiceClosed)
    }

    var firebaseConfig = {
      apiKey: "AIzaSyCpmfng2HzZYH9SiD81CMCcoFx54gKF7IA",
      authDomain: "tongotchi.firebaseapp.com",
      projectId: "tongotchi",
      storageBucket: "tongotchi.appspot.com",
      messagingSenderId: "523191992554",
      appId: "1:523191992554:web:daef77d93dba1f0ebed793"
    };

    // Initialize Firebase
    const app = firebase.initializeApp(firebaseConfig);
    const analytics = firebase.analytics(app);
    //  participation_rates
    console.log(navigator.userAgent)

    function openSharedLink(link) {
      link = "https://t.me/share/url?url=" + link
      Telegram.WebApp.openTelegramLink(link);
    }

    function openLink(link) {
      Telegram.WebApp.openTelegramLink(link);
    }

    function logEvent(eventName, eventParams) {
      console.log(eventName + " | " + eventParams)
      analytics.logEvent(eventName, eventParams)
    }

    function showAds() {
      // insert your block id
      const AdController = window.Adsgram.init({ blockId: "3676" });

      AdController.show().then((result) => {
        // user watch ad till the end
        // your code to reward user
        unityInstance.SendMessage('AdsManager', 'OnShowAdsSuccess', '');
      }).catch((result) => {
        // user skipped video or get error during playing ad
        // do nothing or whatever you want
        unityInstance.SendMessage('AdsManager', 'OnShowAdsFailed', '');
      })
    }

    window.getUser = getUser
    window.openSharedLink = openSharedLink
    window.openLink = openLink
    window.logEvent = logEvent
    window.showAds = showAds

  }).catch((message) => {
    alert(message);
  });
};

function HandleInvoiceClosed() {
  unityInstanceRef.SendMessage('Payment', 'PaymentReceived', "Purchase Done!");
}

document.body.appendChild(script);