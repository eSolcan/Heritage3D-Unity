mergeInto(LibraryManager.library, {
  ChangeHtmlCode: function (htmlCode) {
    window.dispatchReactUnityEvent("ChangeHtmlCode", UTF8ToString(htmlCode));
  },
  GetUsernameAndToken: function () {
    window.dispatchReactUnityEvent("GetUsernameAndToken");
  },
});