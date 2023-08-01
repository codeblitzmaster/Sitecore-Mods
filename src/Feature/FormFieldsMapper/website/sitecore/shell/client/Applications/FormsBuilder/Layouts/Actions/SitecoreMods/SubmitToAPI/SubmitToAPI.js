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
                    debugger;
                    this.MapFieldsTabApp.FormFieldsMapper.setFormFields(formFields);
                    this.MapFieldsTabApp.FormFieldsMapper.setDestinationFields(this.ApiEndpointTabApp.ApiEndpointTreeView.SelectedItem);
                },
                loadDone: function (parameters) {
                    var app = this;
                    //this.Tabs.on("loaded", function () {
                    //    debugger;
                    //});
                    this.Tabs.on("loaded:ApiEndpointTab", function () {
                        app.bindApiEndpointEvents();
                    });
                    this.Tabs.on("loaded:MapFieldsTab", function () {
                        app.setFormFields();
                        debugger
                    });
                    this.Tabs.on("loaded:ReviewTab", function () {
                        debugger;
                    });
                    debugger;
                    //this.setFormFields();
                    this.Parameters = parameters || {};
                },
                bindApiEndpointEvents: function () {
                    this.ApiEndpointTabApp.ApiEndpointTreeView.on("change:SelectedItem", this.changedSelectedItemId, this);
                },
                changedSelectedItemId: function () {
                    var selectedItem = this.ApiEndpointTabApp.ApiEndpointTreeView.SelectedItem;
                    var isSelectable = !!selectedItem;

                    var mapFieldsTab = this.getTab("MapFieldsTab");
                    //enabling MapFieldsTab (Mappings)
                    if (isSelectable && selectedItem.$templateName == "ApiIntegration") {
                        //this.Tabs.toggleEnabledAt(1);
                        mapFieldsTab.IsDisabled = 0;
                        debugger;
                        //logic to enable ok button
                        //parentApp.setSelectability(this, isSelectable, this.ApiEndpointTabApp.ApiEndpointTreeView.SelectedItemId);
                    }
                    else {
                        mapFieldsTab.IsDisabled = 1;
                    }
                },
                getTab: function (name) {
                    var tab = _.filter(this.Tabs.Items, function (item) {
                        return item.$itemName === name;
                    });

                    return tab[0];
                }

            };
        });
})(Sitecore.Speak);