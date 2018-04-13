
const beatPulseIntervalStorageKey = "beatpulse-ui-polling";

Vue.filter('formatDate', function (date) {
    if (date) {
        return new Date(date).toLocaleString();
    }
});

var app = new Vue({
    el: '#app',
    data: {
        livenessData: []
    },
    created: function () {
        this.pollingIntervalSetting = localStorage.getItem(beatPulseIntervalStorageKey) || 10;
    },
    methods: {
        init: function () {
            this.load();
            this.initPolling();
        },
        initPolling: function () {
            localStorage.setItem(beatPulseIntervalStorageKey, this.pollingIntervalSetting);
            BeatPulse.client.startPolling(this._configuredInterval(), this.load.bind(this));
        },
        load: function () {
            var self = this;
            self.livenessData = [];
            BeatPulse.client.healthCheck().then(response => {
                self.livenessData = response.data;
            }).catch(err => self.setError(JSON.stringify(err)));
        },
        _configuredInterval: function () {
            return (localStorage.getItem(beatPulseIntervalStorageKey) || pollingIntervalSetting) * 1000;
        },
        getStatusPic(status) {
            return BeatPulse.client.getStatusImage(status);
        },
        renderBackground(status) {
            return status === BeatPulse.healthStatus.up ? "" : status === BeatPulse.healthStatus.down ? "table-danger" : "table-warning";
        }
    }
});

app.init();

