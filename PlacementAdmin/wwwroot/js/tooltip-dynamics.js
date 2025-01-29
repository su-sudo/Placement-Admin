document.addEventListener('DOMContentLoaded', function (event) {
    var successAlert = document.getElementById('success-alert');
    if (!successAlert) {
        var observer = new MutationObserver(function (mutations) {
            mutations.forEach(function (mutation) {
                if (mutation.addedNodes.length && Array.from(mutation.addedNodes).some(node => node.id === "success-alert")) {
                    var alertNode = mutation.addedNodes[0]; 
                    alertNode.style.display = 'block';
                    setTimeout(function () {
                        alertNode.style.display = 'none';
                    }, 5000);
                }
            });
        });
        observer.observe(document.body, {
            childList: true,
            subtree: true
        });
    } else {
        successAlert.style.display = 'block';
        setTimeout(function () {
            successAlert.style.display = 'none';
        }, 5000);
    }
});
