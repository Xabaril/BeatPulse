
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
    methods: {
        init: function () {
            this.load()
        },
        load: function () {
            var self = this;
            BeatPulse.client.checkHealth().then(response => {
                self.livenessData = response.data;
            }).catch(err => self.setError(JSON.stringify(err)));
        },
        getStatusPic(status) {
            return status === "IsHealthy" ? BeatPulse.resources.okImage : BeatPulse.resources.errorImage;            
        }
    }

});

app.init();