window.uploadlogs = {};
window.uploadlogs.init = function () {
    const initDropArea = function () {
        const dropArea = document.getElementById('upload-logs-drop-area');
        const preventDefaults = function (e) {
            e.preventDefault();
            e.stopPropagation();
        };

        ['dragenter', 'dragover', 'dragleave', 'drop'].forEach(eventName => {
            dropArea.addEventListener(eventName, preventDefaults, false);
        });

        dropArea.addEventListener('drop', handleDrop, false)
        function handleDrop(e) {
            let dt = e.dataTransfer;
            let files = dt.files;
            handleFiles(files);
        };

        function handleFiles(files) {
            if (!files || !files.length) alert("No files");
            if (files.length > 1) alert("Only one file is allowed");
            else uploadFile(files[0]);
        };

        function uploadFile(file) {
            const url = '/api/admin/uploadlogs';
            let formData = new FormData();
            formData.append('file', file);
            const uploadElement = document.getElementById('upload-files');
            uploadElement.className = 'loading';
            fetch(url, {
                    method: 'POST',
                    body: formData
                })
                .then((r) => { uploadElement.className = (r.ok ? 'success' : 'fail'); })
                .catch(() => { uploadElement.className = 'fail'; })
        };

        ['dragenter', 'dragover'].forEach(eventName => {
            dropArea.addEventListener(eventName, function (e) { dropArea.classList.add('active'); }, false)
        });
        ['dragleave', 'drop'].forEach(eventName => {
            dropArea.addEventListener(eventName, function (e) { dropArea.classList.remove('active'); }, false)
        });
    };
    const initUrl = function() {
        document.getElementById('log-file').addEventListener('change',
            function (evt) {
                document.getElementById('upload-logs-submit-with-url').click();
            });

        document.getElementById('upload-logs-click-to-url').addEventListener('click',
            function (evt) {
                const elem = document.getElementById('upload-logs-with-url-container');
                elem.classList.toggle('opened');
                const initialEventTs = evt.timeStamp;
                if (elem.classList.contains('opened')) {
                    const urlOnBlur = function (evt) {
                        evt = evt || window.event;
                        if (evt.timeStamp !== initialEventTs) {
                            const last_target = new ActionsTracker().lastTargets.onClick;
                            if (!(last_target && elem.contains(last_target))) {
                                document.body.removeEventListener('click', urlOnBlur, false);
                                elem.classList.remove('opened');
                            }
                        }
                    };
                    document.body.addEventListener('click', urlOnBlur, false);
                }
            });
    };
    const initFileDialog = function()
    {
        const openButton = document.getElementById('upload-logs-click-to-find');
        openButton.addEventListener('click',
            function (evt) {
                document.getElementById('log-file').click();
            });
    };
    const hideFileForm = function () {
        document.getElementById('upload-logs').style.display = 'none';
    };

    new ActionsTracker();
    initDropArea();
    initFileDialog();
    initUrl();
    hideFileForm();
};
window.uploadlogs.sendAnother = function () {
    const uploadElement = document.getElementById('upload-files');
    uploadElement.className = 'awaiting';
};