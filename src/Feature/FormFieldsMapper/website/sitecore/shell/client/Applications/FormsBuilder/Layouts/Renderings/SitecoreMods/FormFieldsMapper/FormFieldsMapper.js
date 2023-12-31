﻿////(function (Speak) {
////    Speak.component({
////        name: "FormFieldsMapper",
////        initialize: function (app) {
////            //debugger;
////        },
////        getTheListOfComponents: function (app) {
////            var componentsList = '';
////            $(Sitecore.Speak.app.components).each(function () { componentsList += '<li>' + this.id + '</li>'; });
////            $('.sc-CustomComponent[data-sc-id="' + this.id + '"]').html('<ul>' + componentsList + '</ul>');
////        }
////    });

////})(Sitecore.Speak);

(function (Speak) {
    require.config({
        paths: {
            mentionsInput: "/sitecore/shell/client/Applications/FormsBuilder/Layouts/Renderings/SitecoreMods/FormFieldsMapper/js/vendor/jquery.mentionsInput",
            mentionsInputCss: "/sitecore/shell/client/Applications/FormsBuilder/Layouts/Renderings/SitecoreMods/FormFieldsMapper/css/vendor/jquery.mentionsInput"
        }
    });

    var parentApp = window.parent.Sitecore.Speak.app.findApplication('EditActionSubAppRenderer');

    Speak.component(["itemJS", "bclCollection", "mentionsInput", "css!mentionsInputCss"],
        function (itemManager, Collection) {
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
                    //console.log(data);
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

            return Speak.extend({}, Collection.prototype, {
                initialize: function () {
                },
                initialized: function () {
                    Collection.prototype.initialized.call(this);
                    this.on({
                        "loaded": this.loadDone
                    },
                        this);
                    this.defineProperty("FormFields", []);
                    this.defineProperty("DestinationFieldsRootItemId", null);
                    this.defineProperty("DestinationFieldsRootItemName", null);
                    this.defineProperty("HasExistingMappings", false);
                    this.defineProperty("ExistingMappings", null);

                    this.on("update:FieldsMapper", function (data) {
                        if (data.destinationFieldsRootItemId != null && data.destinationFieldsRootItemId != this.DestinationFieldsRootItemId) {
                            this.setFormFields();
                            this.DestinationFieldsRootItemId = data.destinationFieldsRootItemId;
                            this.DestinationFieldsRootItemName = data.destinationFieldsRootItemName;
                            if (data.hasOwnProperty("mappings")) {
                                if (typeof (data.mappings) != "undefined") {
                                    if (data.mappings.length > 0) {
                                        this.ExistingMappings = data.mappings;
                                        this.HasExistingMappings = true;
                                    }
                                }
                            }
                            this.getDestinationFieldsAndBuildTable(this.DestinationFieldsRootItemId);
                            this.validateItems();
                        }
                        if (this.Debug) {
                            this.renderDebugInfo();
                        }
                    });
                },
                loadDone: function () {
                },
                setFormFields: function () {
                    var formFields = this.FormClientApi.getFields();
                    this.FormFields = _.map(formFields, function (field) {
                        return {
                            id: field.itemId,
                            name: field.name
                        };
                    });
                    
                    
                },
                getFormFields: function () {
                    return this.FormFields;
                },
                getDestinationFieldsAndBuildTable: function (destinationFieldsRootItemId) {
                    options = {};
                    options.database = Speak.Context.current().contentDatabase;
                    var app = this;
                    getChildren(destinationFieldsRootItemId, function (items) {
                        var formattedDestinationFields = _.map(items, function (field) {
                            var destinationField = {
                                //"Name": field.$itemName,
                                "Name": field.Name,
                                "DisplayName": field.$displayName,
                                "Description": field.Description,
                                "Type": field.Type,
                                "IsRequired": field.IsRequired == "1" ? true : false,
                                "IsPrimary": field.IsPrimary == "1" ? true : false,
                                "Value": field.DefaultValue,
                                "ValueConverterType": field.ValueConverterType,
                                "ValueConverterTypeParams": field.ValueConverterTypeParams,
                                "ID": field.$itemId
                            };
                            // Logic to update back exiting mapping values on the destination fields before rendering table
                            if (app.HasExistingMappings) {
                                var existingMappedField = _.find(app.ExistingMappings, function (existingField) {
                                    //return existingField.Name == field.$itemName;
                                    return existingField.Name == field.Name;
                                });
                                if (typeof (existingMappedField) != "undefined") {
                                    destinationField.Value = existingMappedField.Value;
                                }
                            }
                            return destinationField;
                        });
                        app.Items = formattedDestinationFields;
                        this.buildTable();
                        this.app.ProgressIndicatorPanel.IsBusy = false;
                    }.bind(this), options);
                },
                buildTable: function () {
                    if (this.Items.length < 1) {
                        if ($('.form-fields-mapper-body .form-fields-mapper-msg').length < 1) {
                            $('.form-fields-mapper-body').append('<p class="form-fields-mapper-msg">No Fields available for mapping</p>');
                        }
                    }
                    else {
                        /*
                            <tr>
                                <td>
                                    <label for="firstname" class="required">FirstName</label>
                                    <span>Firstname of the person</span>
                                </td>
                                <td>
                                    <textarea id="firstname" required></textarea>
                                </td>
                            </tr>
                         */
                        var $tableBody = $("#form-fields-mapper tbody");
                        // clear all rows before adding new ones
                        $tableBody.empty();
                        _.each(this.Items, function (item, idx) {
                            var $tr = $('<tr>');

                            var $destinationColumn = $('<td>');
                            var $destinationColumnLabel = $('<label>', {
                                for: item.Name,
                                "class": item.IsRequired ? "required" : null,
                                text: item.Name
                            });
                            $destinationColumn.append($destinationColumnLabel);
                            if (item.Description != "") {
                                var $destinationColumnDescription = $('<span>', {
                                    "class": "description",
                                    text: item.Description
                                });
                                $destinationColumn.append($destinationColumnDescription);
                            }
                            $tr.append($destinationColumn);

                            var $sourceColumn = $('<td>');
                            var $sourceColumnTextarea = $('<textarea>', {
                                id: item.Name,
                                //text: item.Value,
                                "class": "form-control source-field",
                                required: item.IsRequired,
                                "data-row": idx
                            });
                            $sourceColumnTextarea.val(item.Value);
                            $sourceColumn.append($sourceColumnTextarea);
                            $tr.append($sourceColumn);
                            $tableBody.append($tr);
                        });

                        var formFields = this.getFormFields();

                        var app = this;

                        $tableBody.find(".source-field").each(function (idx,field) {
                            var $field = $(field);
                            var fieldValue = $field.val();
                            $field.mentionsInput({
                                defaultValue: fieldValue,
                                //allowRepeat: true,
                                onDataRequest: function (model, query, callback) {
                                    var data = formFields;
                                    data = _.filter(data, function (item) { return item.name.toLowerCase().indexOf(query.toLowerCase()) > -1 });
                                    callback.call(this, data);
                                },
                                templates: {
                                    mentionItemSyntax: _.template('@[<%= value %>]'),
                                }
                            });
                            $field.on("keyup", _.debounce(function () {
                                var idx = $(this).data("row");
                                $(this).mentionsInput("val", function (newValue) {
                                    //console.log(newValue);
                                    app.Items[idx].Value = newValue;
                                });
                                //console.log("Updated Value:", app.Items[idx].Value);
                                // re-validate items on every change in source field
                                app.validateItems();
                            },500));
                        });
                        // validate items on load
                        app.validateItems();
                    }
                },
                renderDebugInfo: function () {
                    var formFieldsList = '';
                    $(this.FormFields).each(function () { formFieldsList += '<li>' + this.name + ' (' + this.id + ')' + '</li>'; });
                    $('.sc-FormFieldsMapper[data-sc-id="' + this.id + '"] .debug-info').html('<div><p><b>Form Fields</b></p><ul>' + formFieldsList + '</ul></div>');
                    $('.sc-FormFieldsMapper[data-sc-id="' + this.id + '"] .debug-info').append('<p><b>Destination Fields Root: </b>' + this.DestinationFieldsRootItemName + ' (' + this.DestinationFieldsRootItemId + ')' + '</p>');
                    $('.sc-FormFieldsMapper[data-sc-id="' + this.id + '"] .debug-info').append('<div><p><b>Mappings</b></p><pre id="mappings-json"></pre></div>');
                },
                getMappings: function () {
                    return this.Items;
                },
                validateItems: function () {
                    var mappings = this.getMappings();
                    var isValid = false;
                    var isMappingsValid = _.every(mappings,
                        function (fieldMapping) {
                            if (fieldMapping.IsRequired) {
                                return fieldMapping.Value.length > 0;
                            }
                            return true;
                        });
                    isValid = this.DestinationFieldsRootItemId != null && isMappingsValid;
                    parentApp.setSelectability(this, isValid);
                    if (this.Debug) {
                        $('.sc-FormFieldsMapper[data-sc-id="' + this.id + '"] .debug-info #mappings-json').html(JSON.stringify(this.Items, null, 2));
                    }
                },
                showMessage: function (text, type) {
                    var message = {
                        Type: type || "error",
                        Text: text,
                        IsClosable: true,
                        IsTemporary: false
                    };
                    this.MessageBar.add(message);
                },
                afterRender: function () {
                }
            });
        }, "FormFieldsMapper");
})(Sitecore.Speak);