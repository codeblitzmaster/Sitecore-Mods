(function (speak) {
    var parentApp = window.parent.Sitecore.Speak.app.findApplication('EditActionSubAppRenderer');

    speak.pageCode([],
        function () {

            return {
                initialized: function () {
                    this.on({
                        "loaded": this.loadDone
                    },
                        this);

                    if (parentApp) {
                        parentApp.loadDone(this, this.HeaderTitle.Text, this.HeaderSubtitle.Text);
                        //parentApp.loadDone(this, "Submit to API", "Integrate with any API");
                    }
                },

                setFormFields: function () {
                    var formFields = this.FormClientApi.getFields();
                    this.FormFieldsMapper.setFormFields(formFields);
                },

                loadDone: function (parameters) {
                    this.setFormFields();
                    this.Parameters = parameters || {};
                }
            };
        });
})(Sitecore.Speak);