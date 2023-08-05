(function (speak) {
    var parentApp = window.parent.Sitecore.Speak.app.findApplication('EditActionSubAppRenderer');

    speak.pageCode([],
        function () {
            var apiEndpoint = null, apiEndpointId = null;
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
                loadDone: function (parameters) {
                    var app = this;
                    //this.Tabs.on("loaded", function () {
                    //    debugger;
                    //});
                    this.Tabs.on("loaded:ApiEndpointTab", function () {
                        app.bindApiEndpointEvents();
                    });
                    this.Tabs.on("loaded:MapFieldsTab", function () {
                        //app.renderFieldsMapper();
                        debugger;
                    });
                    //Triggered when tab is selected
                    this.Tabs.on("change:SelectedValue", function (selectedTab) {
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
                        apiEndpoint = selectedItem;
                        apiEndpointId = selectedItem.$itemId;

                        //Notify FormFieldsMapper about ApiEndpoint selection change to rerender if selected item is different than previous one
                        Sitecore.Speak.app.MapFieldsTabApp.FormFieldsMapper.trigger("updated:DestinationFieldsRootItem", apiEndpointId);

                        if (mapFieldsTab.IsDisabled = 1) {
                            mapFieldsTab.IsDisabled = 0;
                        }
                    }
                },
                getTab: function (name) {
                    var tab = _.filter(this.Tabs.Items, function (item) {
                        return item.$itemName === name;
                    });

                    return tab[0];
                },
                getData: function () {
                    this.Parameters.Mappings = this.MapFieldsTabApp.FormFieldsMapper.getMappings();
                    this.Parameters.ApiEndpointId = apiEndpointId;
                    debugger;
                    return this.Parameters;
                }
            };
        });
})(Sitecore.Speak);