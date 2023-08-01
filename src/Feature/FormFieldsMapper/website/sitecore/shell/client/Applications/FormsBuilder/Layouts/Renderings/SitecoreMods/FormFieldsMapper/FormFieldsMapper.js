////(function (Speak) {
////    Speak.component({
////        name: "FormFieldsMapper",
////        initialize: function (app) {
////            debugger;
////        },
////        getTheListOfComponents: function (app) {
////            var componentsList = '';
////            $(Sitecore.Speak.app.components).each(function () { componentsList += '<li>' + this.id + '</li>'; });
////            $('.sc-CustomComponent[data-sc-id="' + this.id + '"]').html('<ul>' + componentsList + '</ul>');
////        }
////    });

////})(Sitecore.Speak);

(function (Speak) {

    Speak.component(["itemJS"],
        function (itemManager) {
            var getItem = function (guid, callBack, options) {
                if (!guid) {
                    callBack(null);
                    return;
                }

                options = options || {};
                options.language = Speak.Context.current().Language;
                options.database = Speak.Context.current().contentDatabase;

                itemManager.fetch(guid,
                    options,
                    function (data, result) {
                        if (result.statusCode === 401) {
                            Speak.module("bclSession").unauthorized();
                            return;
                        }
                        callBack(data);
                    });
            };
            var getChildren = function (guid, callBack, options) {
                if (!guid) {
                    callBack(null);
                    return;
                }

                options = options || {};
                options.language = Speak.Context.current().Language;
                options.database = Speak.Context.current().contentDatabase;

                itemManager.fetchChildren(guid, options, function (data, result) {
                    if (result.statusCode === 401) {
                        Speak.module("bclSession").unauthorized();
                        return;
                    }
                    console.log(data);
                    callBack(data);
                });
            };

            var getListItems =
                function (listItems, valueFieldName, displayFieldName, selectedValue, notFoundText) {
                    var currentField = {};
                    currentField[valueFieldName] = selectedValue;
                    if (selectedValue && !_.findWhere(listItems, currentField)) {
                        currentField[displayFieldName] = selectedValue + " - " + notFoundText;
                        currentField.notFound = true;
                        var items = listItems.slice(0);
                        items.unshift(currentField);

                        return items;
                    }

                    return listItems;
                };

            return Speak.extend({}, {
                initialize: function () {
                    debugger;
                    this.defineProperty("FormFields", []);
                    this.defineProperty("DestinationFields", []);

                },
                setFormFields: function (formFields) {
                    this.FormFields = formFields;
                    this.renderFormFields();
                },
                setDestinationFields: function (item) {
                    options = {};
                    options.database = Speak.Context.current().contentDatabase;
                    getChildren(item.$itemId, function (items) {
                        debugger;
                        this.DestinationFields = (items.length > 0) ? items : [];
                    }.bind(this));
                    
                    debugger;
                },
                renderFormFields: function () {
                    var formFieldsList = '';
                    $(this.FormFields).each(function () { formFieldsList += '<li>' + this.name + ' (' + this.itemId + ')' + '</li>'; });
                    $('.sc-FormFieldsMapper[data-sc-id="' + this.id + '"] .debug-info').html('<ul>' + formFieldsList + '</ul>');
                }
            });
        }, "FormFieldsMapper");
}) (Sitecore.Speak);