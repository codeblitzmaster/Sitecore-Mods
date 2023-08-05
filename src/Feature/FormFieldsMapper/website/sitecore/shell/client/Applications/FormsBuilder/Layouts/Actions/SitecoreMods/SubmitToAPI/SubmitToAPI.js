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
                    var app = this;

                    //this.Tabs.on("loaded", function () {
                       
                    //    //debugger;
                    //});

                    this.Tabs.on("loaded:ApiEndpointTab", function () {
                        app.bindApiEndpointEvents();
                        //debugger;
                        app.ApiEndpointTabApp.ApiEndpointTreeView.SelectedItemId = app.Parameters.ApiEndpointId;
                        //app.ApiEndpointTabApp.ApiEndpointTreeView.setSelectedItemId(app.Parameters.ApiEndpointId);
                    });
                    this.Tabs.on("loaded:MapFieldsTab", function () {
                        if (apiEndpoint != null) {
                            var mappings = Sitecore.Speak.app.Parameters.Mappings;
                            //Notify FormFieldsMapper about ApiEndpoint selection change to rerender if selected item is different than previous one
                            var data = {
                                destinationFieldsRootItemId: apiEndpointId,
                                mappings
                            };
                            Sitecore.Speak.app.MapFieldsTabApp.FormFieldsMapper.trigger("update:FieldsMapper", data);
                            Sitecore.Speak.app.enableTab("MapFieldsTab");
                        }
                        //debugger;
                    });

                    ////Triggered when tab is selected
                    //this.Tabs.on("change:SelectedValue", function (selectedTab) {
                    //    //debugger;
                    //});

                    if (parentApp) {
                        parentApp.loadDone(this, this.HeaderTitle.Text, this.HeaderSubtitle.Text);
                        //parentApp.loadDone(this, "Submit to API", "Integrate with any API");
                    }
                },
                loadDone: function (parameters) {
                    //this.setFormFields();
                    this.Parameters = parameters || {};
                    //debugger;

                },
                bindApiEndpointEvents: function () {
                    this.ApiEndpointTabApp.ApiEndpointTreeView.on("change:SelectedItem", this.changedSelectedItemId, this);
                },
                changedSelectedItemId: function () {
                    var selectedItem = this.ApiEndpointTabApp.ApiEndpointTreeView.SelectedItem;
                    var isSelectable = !!selectedItem;

                    if (isSelectable && selectedItem.$templateName == "ApiIntegration") {
                        //this.Tabs.toggleEnabledAt(1);
                        apiEndpoint = selectedItem;
                        apiEndpointId = selectedItem.$itemId;

                        if (Sitecore.Speak.app.hasOwnProperty("MapFieldsTabApp")) {
                            var data = { destinationFieldsRootItemId: apiEndpointId };
                            //Notify FormFieldsMapper about ApiEndpoint selection change to rerender if selected item is different than previous one
                            Sitecore.Speak.app.MapFieldsTabApp.FormFieldsMapper.trigger("update:FieldsMapper", data );
                        }
                        
                        this.enableTab("MapFieldsTab");
                    }
                },
                getTab: function (name) {
                    var tab = _.filter(this.Tabs.Items, function (item) {
                        return item.$itemName === name;
                    });

                    return tab[0];
                },
                enableTab: function (name) {
                    var tab = this.getTab(name);
                    if (tab.IsDisabled == 1) {
                        tab.IsDisabled = 0;
                    }
                },
                getData: function () {
                    this.Parameters.Mappings = this.MapFieldsTabApp.FormFieldsMapper.getMappings();
                    this.Parameters.ApiEndpointId = apiEndpointId;
                    //debugger;
                    return this.Parameters;
                }
            };
        });
})(Sitecore.Speak);